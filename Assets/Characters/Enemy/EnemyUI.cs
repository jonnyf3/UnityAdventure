using UnityEngine;

namespace RPG.Characters
{
    public class EnemyUI : MonoBehaviour
    {
        Camera cameraToLookAt;

        void Start() {
            cameraToLookAt = Camera.main;
        }

        // Rotate the canvas so the health bar is always pointed at the camera
        void LateUpdate() {
            transform.LookAt(cameraToLookAt.transform);
            transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        }
    }
}