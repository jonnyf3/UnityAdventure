using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    public class Player : MonoBehaviour
    {
        CameraController camera = null;
        PlayerAttack attackComponent = null;
        ThirdPersonCharacter character = null;
        Health health = null;

        public delegate void OnPlayerDied();
        public event OnPlayerDied onPlayerDied;

        // Start is called before the first frame update
        void Start() {
            camera = GetComponent<CameraController>();
            attackComponent = GetComponent<PlayerAttack>();
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

        public void Attack() {
            attackComponent.Attack();
        }

        private void LateUpdate() {
            //Move command causes the body to move relative to the main Player object
            //Ensure camera follows player and reset local positon to zero
            transform.position += character.transform.localPosition; ;
            character.transform.localPosition = Vector3.zero;
        }
        
        private void Die() {
            print("Player died!");
            //TODO destroy game object?
            onPlayerDied();
        }
    }
}