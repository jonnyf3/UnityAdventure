using UnityEngine;
using RPG.Characters;

namespace RPG.Core
{
    public class InputHandler : MonoBehaviour
    {
        private Player player;

        private void Start() {
            player = GameObject.FindObjectOfType<Player>();
        }

        // Process any input
        void FixedUpdate() {
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
                player.Heal();
            }
        }
    }
}