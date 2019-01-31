using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class BackupCamera : MonoBehaviour
    {
        private Camera camera;

        // Start is called before the first frame update
        void Start() {
            camera = GetComponent<Camera>();
            if (camera.isActiveAndEnabled) { camera.enabled = false; }

            var player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(player, "Could not find player in the scene!");
            player.GetComponent<Player>().onPlayerDied += MakeActiveCamera;
        }

        void MakeActiveCamera() {
            //camera.enabled = true;
        }
    }
}