using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.UI
{
    public class BackupCamera : MonoBehaviour
    {
        private new Camera camera = null;

        // Start is called before the first frame update
        void Start() {
            camera = GetComponent<Camera>();
            if (camera.isActiveAndEnabled) { camera.enabled = false; }

            var player = FindObjectOfType<PlayerController>();
            Assert.IsNotNull(player, "Could not find player in the scene!");
            player.onDeath += MakeActiveCamera;
        }

        void MakeActiveCamera() {
            //camera.enabled = true;
        }
    }
}