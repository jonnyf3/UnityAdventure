using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.SceneManagement;
using RPG.Control;

namespace RPG.UI
{
    public class MainMenu : MonoBehaviour
    {
        private const string ENTER_BUTTON = "X";

        private List<Button> buttons;
        private Button activeButton;

        private void Start() {
            buttons = new List<Button>(GetComponentsInChildren<Button>());
            activeButton = buttons[0];
        }

        private void Update() {
            if (ControllerInput.GetVerticalButtonsDown(out int direction)) {
                var newIndex = (buttons.IndexOf(activeButton) + direction) % buttons.Count;
                activeButton = buttons[newIndex];
            }

            if (Input.GetButtonDown(ENTER_BUTTON)) { activeButton.onClick.Invoke(); }
        }

        public void NewGame() {
            FindObjectOfType<SceneController>().LoadLevel(SceneController.SCENE_1);
        }
        public void LoadGame() {
            //TODO
        }
        public void Quit() {
            Application.Quit();
        }
    }
}