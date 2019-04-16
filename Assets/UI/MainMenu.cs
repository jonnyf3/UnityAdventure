using UnityEngine;
using RPG.SceneManagement;
using RPG.Saving;

namespace RPG.UI
{
    public class MainMenu : MonoBehaviour
    {
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