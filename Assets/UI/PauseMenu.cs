using UnityEngine;
using RPG.Saving;
using RPG.SceneManagement;
using RPG.Control;

namespace RPG.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private const string DISMISS_BUTTON = "Circle";
        private void Update() {
            if (Input.GetButtonDown(DISMISS_BUTTON) || Input.GetButtonDown(ControllerInput.PAUSE_BUTTON)) {
                Resume();
            }
        }

        public void Resume() {
            FindObjectOfType<SceneController>().Resume();
        }
        public void SaveGame() {
            FindObjectOfType<SaveManager>().Save();
        }
        public void LoadGame() {
            FindObjectOfType<SaveManager>().Load();
        }
        public void Quit() {
            FindObjectOfType<SceneController>().LoadLevel(SceneController.MAIN_MENU);
        }
    }
}