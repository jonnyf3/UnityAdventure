using UnityEngine;

namespace RPG.Characters
{
    public class CharacterUI : MonoBehaviour
    {
        // Align the canvas so the health bar is always pointed at the camera
        void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }
}