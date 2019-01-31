using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
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
            player.RotateCamera(CrossPlatformInputManager.GetAxis("CameraX"),
                                CrossPlatformInputManager.GetAxis("CameraY"));

            player.Move(CrossPlatformInputManager.GetAxis("Vertical"),
                        CrossPlatformInputManager.GetAxis("Horizontal"));

            if (CrossPlatformInputManager.GetButtonDown("Square")) {
                player.Attack();
            }
        }
    }
}