using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Movement;
using RPG.Combat;
using RPG.Actions;
using RPG.UI;

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

        //passed arguments
        private Transform projectileSpawn;
        private Transform abilitySpawn;

        private float colliderOriginalHeight;
        private Vector3 colliderOriginalCenter;

        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

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

        public override void SetArgs(StateArgs args) {
            base.SetArgs(args);

            var controlArgs = args as ControlledStateArgs;
            this.projectileSpawn = controlArgs.projectileSpawn;
            this.abilitySpawn = controlArgs.abilitySpawn;
        }

        private void Update() {
            ProcessMovement(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            ProcessCameraMovement(Input.GetAxis("CameraX"), Input.GetAxis("CameraY"));

            if (Input.GetButtonDown("LeftTrigger")) {
                StartCoroutine(Focus());
            }

            //Only attack (or roll) if not already doing so
            //TODO can still double tap
            bool alreadyAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
            if (!alreadyAttacking) {
                if (Input.GetButtonDown("RightShoulder")) {
                    AlignSpawnPoint(projectileSpawn);
                    combat.Attack();
                }
                if (Input.GetButtonDown("RightTrigger")) {
                    AlignSpawnPoint(abilitySpawn);
                    abilities.Use();
                }
                if (Input.GetButtonDown("Circle")) {
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
            while (Input.GetButton("LeftTrigger")) {
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                yield return new WaitForEndOfFrame();
            }

            Camera.main.fieldOfView = currentFOV;
            movement.Focussed = false;
        }

        private IEnumerator Roll() {
            animator.SetTrigger("onRoll");

            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight / 2f;
            collider.center -= new Vector3(0, colliderOriginalHeight / 4f, 0f);
            yield return new WaitForSeconds(1f);

            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }

        private void ProcessWeaponToggle() {
            if (GetVerticalButtonsDown()) {
                var verticalButtonDirection = (int)Mathf.Sign(lastFrameDPADvertical);
                combat.CycleWeapon(verticalButtonDirection);
            }
            if (GetHorizontalButtonsDown()) {
                var horizontalButtonDirection = (int)Mathf.Sign(lastFrameDPADhorizontal);
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

        public override void OnStateExit() {
            //catch in case of state exit mid-roll
            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }

        //Wrapper methods to make DPAD axes behave like discrete buttons
        private float lastFrameDPADhorizontal = 0;
        private float lastFrameDPADvertical = 0;

        private bool GetVerticalButtonsDown() {
            var thisFrameDPADvertical = Input.GetAxis("DpadVertical");

            //Button press same as last frame
            if (thisFrameDPADvertical == lastFrameDPADvertical) {
                return false;
            }
            //Button not pressed this frame
            if (thisFrameDPADvertical == 0) {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }
            //Button press same direction as this frame (but different size)
            if (Mathf.Sign(thisFrameDPADvertical) == Mathf.Sign(lastFrameDPADvertical) && lastFrameDPADvertical != 0) {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }

            //Any other case should return true
            lastFrameDPADvertical = thisFrameDPADvertical;
            return true;
        }
        private bool GetHorizontalButtonsDown() {
            var thisFrameDPADhorizontal = Input.GetAxis("DpadHorizontal");

            //Button press same as last frame
            if (thisFrameDPADhorizontal == lastFrameDPADhorizontal) {
                return false;
            }
            //Button not pressed this frame
            if (thisFrameDPADhorizontal == 0) {
                lastFrameDPADhorizontal = thisFrameDPADhorizontal;
                return false;
            }
            //Button press same direction as this frame (but different size)
            if (Mathf.Sign(thisFrameDPADhorizontal) == Mathf.Sign(lastFrameDPADhorizontal) && lastFrameDPADhorizontal != 0) {
                lastFrameDPADhorizontal = thisFrameDPADhorizontal;
                return false;
            }

            //Any other case should return true
            lastFrameDPADhorizontal = thisFrameDPADhorizontal;
            return true;
        }
    }

    public class ControlledStateArgs : StateArgs
    {
        public Transform projectileSpawn;
        public Transform abilitySpawn;

        public ControlledStateArgs(Player character, Transform projectileSpawn, Transform abilitySpawn) : base(character)
        {
            this.projectileSpawn = projectileSpawn;
            this.abilitySpawn = abilitySpawn;
        }
    }
}