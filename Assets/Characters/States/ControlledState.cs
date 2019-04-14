using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;
using RPG.Actions;
using RPG.UI;
using RPG.Control;
using RPG.Saving;

namespace RPG.States
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(CombatSystem))]
    [RequireComponent(typeof(SpecialAbilities))]
    public class ControlledState : State
    {
        //required components
        CombatSystem     combat;
        WeaponSystem     weapons;
        SpecialAbilities abilities;
        Animator         animator;
        CameraController camera;

        private Transform projectileSpawn => (character as Player).projectileSpawn;
        private Transform abilitySpawn    => (character as Player).abilitySpawn;

        private float   colliderOriginalHeight;
        private Vector3 colliderOriginalCenter;

        public const string ANIMATOR_ROLL_PARAM = "onRoll";

        protected override void Awake() {
            base.Awake();
            Assert.IsNotNull(character as Player, "ControlledState should only be entered by a Player character");

            var collider = GetComponent<CapsuleCollider>();
            colliderOriginalHeight = collider.height;
            colliderOriginalCenter = collider.center;
        }

        private void Start() {
            combat = GetComponent<CombatSystem>();
            weapons = GetComponent<WeaponSystem>();
            abilities = GetComponent<SpecialAbilities>();
            animator = GetComponent<Animator>();
            camera = GetComponent<CameraController>();
        }

        private void Update() {
            if (!character.IsOnGround) { character.SetState<FallingState>(); return; }

            ProcessMovement(Input.GetAxis(ControllerInput.MOVE_Y_AXIS),
                            Input.GetAxis(ControllerInput.MOVE_X_AXIS));
            
            ProcessCameraMovement(Input.GetAxis(ControllerInput.CAMERA_X_AXIS),
                                  Input.GetAxis(ControllerInput.CAMERA_Y_AXIS));

            if (Input.GetButtonDown(ControllerInput.FOCUS_BUTTON)) {
                StartCoroutine(Focus());
            }

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
            if (Input.GetButtonDown(ControllerInput.SHOW_UI_BUTTON)) {
                FindObjectOfType<HUD>().ShowAllUI();
            }
            //TODO implement saving via UI menu
            if (Input.GetKeyDown(KeyCode.S)) {
                FindObjectOfType<SaveManager>().Save();
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                FindObjectOfType<SaveManager>().Load();
            }

            ProcessWeaponToggle();
        }

        private void ProcessMovement(float forward, float right) {
            // Get player controller input direction relative to camera direction
            var cameraRelative = forward * camera.Forward + right * camera.Right;
            character.Move(transform.position + cameraRelative.normalized);
        }

        private void ProcessCameraMovement(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        private IEnumerator Focus() {
            character.Focus(true);
            camera.EnableFocusCamera();

            //Focus on viewpoint target while trigger is held down
            while (Input.GetButton(ControllerInput.FOCUS_BUTTON)) {
                transform.forward = camera.Forward;
                yield return new WaitForEndOfFrame();
            }
            
            character.Focus(false);
            camera.DisableFocusCamera();
        }

        private IEnumerator Roll() {
            animator.SetTrigger(ANIMATOR_ROLL_PARAM);

            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight / 2f;
            collider.center -= new Vector3(0, colliderOriginalHeight / 4f, 0f);
            yield return new WaitForSeconds(0.8f);

            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }

        private void ProcessWeaponToggle() {
            if (ControllerInput.GetVerticalButtonsDown(out int v)) { weapons.CycleWeapon(v); }
            if (ControllerInput.GetHorizontalButtonsDown(out int h)) { abilities.CycleAbilities(h); }
        }

        private void AlignSpawnPoint(Transform spawnPoint) {
            if (!spawnPoint) { return; }

            if (animator.GetBool("isFocussed")) {
                spawnPoint.LookAt(camera.LookTarget);
            } else {
                spawnPoint.forward = transform.forward;
            }
        }

        private void OnDestroy() {
            //catch in case of state exit mid-roll
            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;

            //catch in case of entering cutscene while focussing
            character.Focus(false);
            if (camera) { camera.DisableFocusCamera(); }
        }
    }
}