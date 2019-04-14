using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.SceneManagement;
using RPG.Control;
using RPG.Saving;

namespace RPG.UI
{
    public class MainMenu : MonoBehaviour
    {
        private const string ENTER_BUTTON = "X";

        private List<Button> buttons;
        private Button activeButton;
        [SerializeField] Image activeButtonIcon = null;

        private void Start() {
            buttons = new List<Button>(GetComponentsInChildren<Button>());
            activeButton = buttons[0];
            MovePointer();
        }

        private void Update() {
            if (ControllerInput.GetVerticalButtonsDown(out int direction)) {
                var newIndex = (buttons.IndexOf(activeButton) - direction) % buttons.Count;
                activeButton = buttons[newIndex];
                MovePointer();
            }

            if (Input.GetButtonDown(ENTER_BUTTON)) { activeButton.onClick.Invoke(); }
        }
        private void MovePointer() {
            activeButtonIcon.transform.SetParent(activeButton.transform);
            activeButtonIcon.transform.localPosition = new Vector2(-70f, 0f);
        }

        public void NewGame() {
            FindObjectOfType<SceneController>().LoadLevel(SceneController.SCENE_1);
        }
        public void LoadGame() {
            FindObjectOfType<SaveManager>().Load();
        }
        public void Quit() {
            print("Quit game");
            Application.Quit();
        }
    }
}