using UnityEngine;
using RPG.Characters;
using System;

namespace RPG.Core
{
    public class InputHandler : MonoBehaviour
    {
        private Player player;
        private bool isPlayerDead = false;

        private void Start() {
            player = GameObject.FindObjectOfType<Player>();
            player.onPlayerDied += OnPlayerDied;
        }

        // Process any input
        void FixedUpdate() {
            if (isPlayerDead) {
                if (Input.GetButtonDown("X")) {
                    player.Respawn();
                    isPlayerDead = false;
                }
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
            if (Input.GetButtonDown("Triangle")) {
                //TODO map this properly to gamepad arrow buttons
                player.gameObject.GetComponent<PlayerCombat>().CycleMagic();
            }
        }

        private void OnPlayerDied() {
            isPlayerDead = true;
        }
    }
}