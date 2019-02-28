using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Combat;
using RPG.Actions;
using RPG.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(SpecialAbilities))]
    public class PlayerController : Character
    {
        private new CameraController camera;
        private Viewer viewer;
        private WeaponSystem combat;
        private SpecialAbilities abilities;
        
        private Transform projectileSpawn;
        private Transform abilitySpawn;

        private float lastFrameDPADhorizontal = 0;
        private float lastFrameDPADvertical = 0;
        private float colliderOriginalHeight;
        private Vector3 colliderOriginalCenter;

        protected override void Start() {
            base.Start();

            allyState = AllyState.Ally;

            camera = GetComponent<CameraController>();
            viewer = FindObjectOfType<Viewer>();
            Assert.IsNotNull(viewer, "There is no camera Viewer to focus on");

            combat = GetComponent<WeaponSystem>();
            abilities = GetComponent<SpecialAbilities>();

            var collider = GetComponent<CapsuleCollider>();
            colliderOriginalHeight = collider.height;
            colliderOriginalCenter = collider.center;
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
            movement.Move(cameraRelative.normalized, focussed);
        }

        private void ProcessCameraMovement(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        private void AlignSpawnPoint(Transform spawnPoint) {
            if (!spawnPoint) { return; }

            if (focussed) {
                spawnPoint.LookAt(viewer.LookTarget);
            } else {
                spawnPoint.forward = transform.forward;
            }
        }

        private IEnumerator Focus() {
            var currentFOV = Camera.main.fieldOfView;

            //Focus on viewpoint target while trigger is held down
            Camera.main.fieldOfView = 40;
            Focus(true);
            while(Input.GetButton("LeftTrigger")) {
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                yield return new WaitForEndOfFrame();
            }

            Camera.main.fieldOfView = currentFOV;
            Focus(false);
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

        //Wrapper methods to make DPAD axes behave like discrete buttons
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

        public void SetRangedSpawnPoint(Transform spawnPoint) { projectileSpawn = spawnPoint; }
        public void SetAbilitySpawnPoint(Transform spawnPoint)  { abilitySpawn = spawnPoint; }
    }
}