using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Movement;
using RPG.Combat;
using RPG.Actions;
using RPG.UI;
using RPG.Control;

namespace RPG.States
{
    public class ControlledState : State
    {
        //required components
        WeaponSystem     combat;
        SpecialAbilities abilities;
        CharacterMovement movement;
        new CameraController camera;
        Viewer           viewer;
        Animator         animator;

        private Transform projectileSpawn => (character as Player).projectileSpawn;
        private Transform abilitySpawn    => (character as Player).abilitySpawn;

        private float colliderOriginalHeight;
        private Vector3 colliderOriginalCenter;

        public const string ANIMATOR_ROLL_PARAM = "onRoll";

        public override void Start() {
            base.Start();
            Assert.IsNotNull(character as Player, "ControlledState should only be entered by a Player character");

            animator = GetComponent<Animator>();
            movement = GetComponent<CharacterMovement>();
            combat = GetComponent<WeaponSystem>();
            abilities = GetComponent<SpecialAbilities>();

            camera = GetComponent<CameraController>();
            viewer = FindObjectOfType<Viewer>();
            Assert.IsNotNull(viewer, "There is no camera Viewer to focus on");

            var collider = GetComponent<CapsuleCollider>();
            colliderOriginalHeight = collider.height;
            colliderOriginalCenter = collider.center;
        }

        private void Update() {
            ProcessMovement(Input.GetAxis(ControllerInput.MOVE_Y_AXIS),
                            Input.GetAxis(ControllerInput.MOVE_X_AXIS));

            ProcessCameraMovement(Input.GetAxis(ControllerInput.CAMERA_X_AXIS),
                                  Input.GetAxis(ControllerInput.CAMERA_Y_AXIS));

            if (Input.GetButtonDown(ControllerInput.FOCUS_BUTTON)) {
                StartCoroutine(Focus());
            }

            //Only attack (or roll) if not already doing so
            //TODO can still double tap
            bool alreadyAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
            if (!alreadyAttacking) {
                if (Input.GetButtonDown(ControllerInput.ATTACK_BUTTON)) {
                    AlignSpawnPoint(projectileSpawn);
                    combat.Attack();
                }
                if (Input.GetButtonDown(ControllerInput.ABILITY_BUTTON)) {
                    AlignSpawnPoint(abilitySpawn);
                    abilities.Use();
                }
                if (Input.GetButtonDown(ControllerInput.ROLL_BUTTON)) {
                    StartCoroutine(Roll());
                }
            }

            ProcessWeaponToggle();
        }

        private void ProcessMovement(float forward, float right) {
            // Get player controller input direction relative to camera direction
            var cameraRelative = forward * camera.Forward + right * camera.Right;
            movement.Move(cameraRelative.normalized);
        }

        private void ProcessCameraMovement(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        private IEnumerator Focus() {
            var currentFOV = Camera.main.fieldOfView;

            //Focus on viewpoint target while trigger is held down
            Camera.main.fieldOfView = 40;
            movement.Focussed = true;
            while (Input.GetButton(ControllerInput.FOCUS_BUTTON)) {
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                yield return new WaitForEndOfFrame();
            }

            Camera.main.fieldOfView = currentFOV;
            movement.Focussed = false;
        }

        private IEnumerator Roll() {
            animator.SetTrigger(ANIMATOR_ROLL_PARAM);

            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight / 2f;
            collider.center -= new Vector3(0, colliderOriginalHeight / 4f, 0f);
            yield return new WaitForSeconds(1f);

            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }

        private void ProcessWeaponToggle() {
            if (ControllerInput.GetVerticalButtonsDown()) {
                var verticalButtonDirection = (int)Mathf.Sign(Input.GetAxis(ControllerInput.DPAD_Y_AXIS));
                combat.CycleWeapon(verticalButtonDirection);
            }
            if (ControllerInput.GetHorizontalButtonsDown()) {
                var horizontalButtonDirection = (int)Mathf.Sign(Input.GetAxis(ControllerInput.DPAD_X_AXIS));
                abilities.CycleAbilities(horizontalButtonDirection);
            }
        }

        private void AlignSpawnPoint(Transform spawnPoint) {
            if (!spawnPoint) { return; }

            if (movement.Focussed) {
                spawnPoint.LookAt(viewer.LookTarget);
            } else {
                spawnPoint.forward = transform.forward;
            }
        }

        public void OnDestroy() {
            //catch in case of state exit mid-roll
            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }
    }
}