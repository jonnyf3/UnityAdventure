using System;
using UnityEngine;
using RPG.Characters;

namespace RPG.Core
{
    public class InputHandler : MonoBehaviour
    {
        private Player player;
        private bool isPlayerDead = false;

        float lastFrameDPADhorizontal = 0;
        float lastFrameDPADvertical = 0;

        private void Start() {
            player = GameObject.FindObjectOfType<Player>();
            player.onDeath += OnPlayerDied;
        }

        // Process any input
        void FixedUpdate() {
            if (isPlayerDead) {
                PlayerDeathUpdate();
                return;
            }

            player.RotateCamera(Input.GetAxis("CameraX"),
                                Input.GetAxis("CameraY"));

            player.Move(Input.GetAxis("Vertical"),
                        Input.GetAxis("Horizontal"));

            //TODO sometimes records multiple presses
            if (Input.GetButtonDown("Square")) {
                player.MeleeAttack();
            }
            if (Input.GetButtonDown("Circle")) {
                player.MagicAttack();
            }

            if (GetVerticalButtonsDown()) {
                var verticalButtonDirection = Math.Sign(lastFrameDPADvertical);
                player.gameObject.GetComponent<WeaponSystem>().CycleWeapon(verticalButtonDirection);
            }
            if (GetHorizontalButtonsDown()) {
                var horizontalButtonDirection = Math.Sign(lastFrameDPADhorizontal);
                player.gameObject.GetComponent<SpecialCombat>().CycleMagic(horizontalButtonDirection);
            }
        }

        //Wrapper methods to make DPAD axes behave like discrete buttons
        private bool GetHorizontalButtonsDown() {
            var thisFrameDPADhorizontal = Input.GetAxis("DpadHorizontal");

            //Button not pressed this frame
            if (thisFrameDPADhorizontal == 0) {
                lastFrameDPADhorizontal = thisFrameDPADhorizontal;
                return false;
            }
            //Button press same as this frame
            if (thisFrameDPADhorizontal == lastFrameDPADhorizontal) {
                return false;
            }
            //Button press same direction as this frame (but different size)
            if ((thisFrameDPADhorizontal > 0 && lastFrameDPADhorizontal > 0) ||
                (thisFrameDPADhorizontal < 0 && lastFrameDPADhorizontal < 0))
            {
                lastFrameDPADhorizontal = thisFrameDPADhorizontal;
                return false;
            }

            //Any other case should return true
            lastFrameDPADhorizontal = thisFrameDPADhorizontal;
            return true;
        }
        private bool GetVerticalButtonsDown() {
            var thisFrameDPADvertical = Input.GetAxis("DpadVertical");

            //Button not pressed this frame
            if (thisFrameDPADvertical == 0) {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }
            //Button press same as this frame
            if (thisFrameDPADvertical == lastFrameDPADvertical) {
                return false;
            }
            //Button press same direction as this frame (but different size)
            if ((thisFrameDPADvertical > 0 && lastFrameDPADvertical > 0) ||
                (thisFrameDPADvertical < 0 && lastFrameDPADvertical < 0))
            {
                lastFrameDPADvertical = thisFrameDPADvertical;
                return false;
            }

            //Any other case should return true
            lastFrameDPADvertical = thisFrameDPADvertical;
            return true;
        }

        private void OnPlayerDied() {
            isPlayerDead = true;
        }
        private void PlayerDeathUpdate() {
            if (Input.GetButtonDown("X")) {
                player.Respawn();
                isPlayerDead = false;
            }
        }
    }
}