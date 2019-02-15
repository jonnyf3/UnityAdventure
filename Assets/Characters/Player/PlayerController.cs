using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(CameraController))]
    public class PlayerController : Character
    {
        private new CameraController camera;
        private WeaponSystem combat;
        private SpecialCombat specialCombat;

        private bool focussed = false;
        private Transform projectileSpawn;
        private Transform magicSpawn;

        private float lastFrameDPADhorizontal = 0;
        private float lastFrameDPADvertical = 0;

        protected override void Start() {
            base.Start();

            camera = GetComponent<CameraController>();
            combat = GetComponent<WeaponSystem>();
            specialCombat = GetComponent<SpecialCombat>();
        }

        private void Update() {
            if (health.IsDead) {
                if (Input.GetButtonDown("X")) { Respawn(); }
                return;
            }

            ProcessMovement(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            ProcessCameraMovement(Input.GetAxis("CameraX"), Input.GetAxis("CameraY"));

            //TODO sometimes records multiple presses
            if (Input.GetButtonDown("Square")) {
                combat.Attack();
            }
            if (Input.GetButtonDown("Circle")) {
                specialCombat.UseMagic();
            }

            if (Input.GetButtonDown("LeftTrigger")) {
                StartCoroutine(Focus());
            }

            if (GetVerticalButtonsDown()) {
                var verticalButtonDirection = (int)Mathf.Sign(lastFrameDPADvertical);
                combat.CycleWeapon(verticalButtonDirection);
            }
            if (GetHorizontalButtonsDown()) {
                var horizontalButtonDirection = (int)Mathf.Sign(lastFrameDPADhorizontal);
                specialCombat.CycleMagic(horizontalButtonDirection);
            }
        }

        private void ProcessMovement(float forward, float right) {
            // Get player controller input direction relative to camera direction
            var cameraRelative = forward * camera.Forward + right * camera.Right;
            movement.Move(cameraRelative, false, focussed);
        }

        private void ProcessCameraMovement(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        private IEnumerator Focus() {
            var viewer = FindObjectOfType<Viewer>();
            Assert.IsNotNull(viewer, "There is no camera Viewer to focus on");
            var currentFOV = Camera.main.fieldOfView;

            //Focus on viewpoint target while trigger is held down
            Camera.main.fieldOfView = 40;
            focussed = true;
            while(Input.GetButton("LeftTrigger")) {
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                if (projectileSpawn) { projectileSpawn.LookAt(viewer.LookTarget); }
                if (magicSpawn)      { magicSpawn.LookAt(viewer.LookTarget); }
                yield return new WaitForEndOfFrame();
            }

            Camera.main.fieldOfView = currentFOV;
            focussed = false;
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
            if (Mathf.Sign(thisFrameDPADhorizontal) == Mathf.Sign(lastFrameDPADhorizontal)) {
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
            if (Mathf.Sign(thisFrameDPADvertical) == Mathf.Sign(lastFrameDPADvertical)) {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }

            //Any other case should return true
            lastFrameDPADvertical = thisFrameDPADvertical;
            return true;
        }

        public void SetRangedSpawnPoint(Transform spawnPoint) { projectileSpawn = spawnPoint; }
        public void SetMagicSpawnPoint(Transform spawnPoint)  { magicSpawn = spawnPoint; }

        public override void Die() {
            base.Die();
            //TODO reload level? Rather than manual respawn
            //StartCoroutine(ReloadLevel());
        }

        private IEnumerator ReloadLevel() {
            yield return new WaitForSecondsRealtime(5f);
            SceneManager.LoadScene(0);
        }

        //Currently manually respawning on the spot rather than using level reload
        public void Respawn() {
            animator.SetTrigger("onRespawn");
            health.RestoreHealth(100f);
        }
    }
}