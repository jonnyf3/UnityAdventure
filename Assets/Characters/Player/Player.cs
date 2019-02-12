using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using RPG.CameraUI;
using RPG.Weapons;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    public class Player : Character
    {
        new CameraController camera;
        WeaponSystem combat;
        SpecialCombat specialCombat;
        
        public delegate void OnPlayerDied();
        public event OnPlayerDied onPlayerDied;

        protected override void Start() {
            base.Start();

            camera = GetComponent<CameraController>();
            combat = GetComponent<WeaponSystem>();
            specialCombat = GetComponent<SpecialCombat>();
        }

        public void Move(float forward, float right) {
            // Get player controller input direction relative to camera direction
            var cameraRelative = forward * camera.Forward + right * camera.Right;
            movement.Move(cameraRelative, false);
        }

        public void RotateCamera(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        public void MeleeAttack() {
            combat.Attack();
        }
        public void MagicAttack() {
            specialCombat.UseMagic();
        }
        
        public void GiveWeapon(WeaponData weapon) {
            combat.UnlockWeapon(weapon);
        }
        
        protected override void Die() {
            onPlayerDied();
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