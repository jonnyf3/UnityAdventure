using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using RPG.CameraUI;
using RPG.Weapons;
using System;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(PlayerCombat))]
    public class Player : MonoBehaviour
    {
        new CameraController camera = null;
        PlayerCombat playerCombat = null;
        PlayerMovement movement = null;
        Health health = null;

        public delegate void OnPlayerDied();
        public event OnPlayerDied onPlayerDied;

        // Start is called before the first frame update
        void Start() {
            camera = GetComponent<CameraController>();
            playerCombat = GetComponent<PlayerCombat>();
            movement = GetComponent<PlayerMovement>();

            health = GetComponent<Health>();
            health.onDeath += Die;
        }

        public void Move(float forward, float right) {
            Vector3 moveVector = camera.Forward * forward + camera.Right * right;
            movement.Move(moveVector, false);
        }

        public void RotateCamera(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        public void MeleeAttack() {
            playerCombat.UseWeapon();
        }
        public void MagicAttack() {
            playerCombat.UseMagic();
        }

        public void GiveWeapon(Weapon weapon) {
            playerCombat.AddWeapon(weapon);
        }
        
        private void Die() {
            onPlayerDied();

            var animator = GetComponentInChildren<Animator>();
            animator.SetBool("isDead", true);
            
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
            animator.SetBool("isDead", false);

            health.RestoreHealth(100f);
        }
    }
}