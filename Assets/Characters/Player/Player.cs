﻿using UnityEngine;
using RPG.CameraUI;
using System;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(PlayerCombat))]
    public class Player : MonoBehaviour
    {
        new CameraController camera = null;
        PlayerCombat playerCombat = null;
        ThirdPersonCharacter character = null;
        Health health = null;

        public delegate void OnPlayerDied();
        public event OnPlayerDied onPlayerDied;

        // Start is called before the first frame update
        void Start() {
            camera = GetComponent<CameraController>();
            playerCombat = GetComponent<PlayerCombat>();
            character = GetComponentInChildren<ThirdPersonCharacter>();

            health = GetComponent<Health>();
            health.onDeath += Die;
        }

        public void Move(float forward, float right) {
            Vector3 movement = camera.Forward * forward + camera.Right * right;
            character.Move(movement, false, false);
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

        private void LateUpdate() {
            //Move command causes the body to move relative to the main Player object
            //Ensure camera follows player and reset local positon to zero
            transform.position = character.transform.position;
            character.transform.localPosition = Vector3.zero;
        }
        
        private void Die() {
            onPlayerDied();

            var animator = GetComponentInChildren<Animator>();
            animator.SetBool("isDead", true);
        }

        public void Respawn() {
            //TODO reload level?
            var animator = GetComponentInChildren<Animator>();
            animator.SetBool("isDead", false);

            health.RestoreHealth(100f);
        }
    }
}