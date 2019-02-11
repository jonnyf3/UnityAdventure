using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using RPG.CameraUI;
using RPG.Weapons;
using RPG.Magic;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(PlayerCombat))]
    public class Player : CharacterController
    {
        new CameraController camera;
        PlayerCombat playerCombat;
        SpecialCombat specialCombat;
        
        public delegate void OnPlayerDied();
        public event OnPlayerDied onPlayerDied;
        
        void Start() {
            //Inherited from CharacterController
            movement = GetComponent<PlayerMovement>();

            //Player specific
            camera = GetComponent<CameraController>();
            playerCombat = GetComponent<PlayerCombat>();
            specialCombat = GetComponent<SpecialCombat>();

            //Event listeners
            health.onDeath += Die;
        }

        public void Move(float forward, float right) {
            // Get player controller input direction relative to camera direction
            var cameraRelative = forward * camera.Forward + right * camera.Right;
            // Convert this into a world-relative direction to pass to PlayerMovement
            var worldRelative = transform.TransformVector(cameraRelative);
            movement.Move(worldRelative, false);
        }

        public void RotateCamera(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        public void MeleeAttack() {
            playerCombat.UseWeapon();
        }
        public void MagicAttack() {
            specialCombat.UseMagic();
        }
        
        public void GiveWeapon(Weapon weapon) {
            playerCombat.AddWeapon(weapon);
        }
        
        private void Die() {
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
            var animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("onRespawn");

            health.RestoreHealth(100f);
        }
    }
}