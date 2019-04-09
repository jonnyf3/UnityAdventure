using UnityEngine;
using RPG.SceneManagement;

namespace RPG.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void NewGame() {
            SceneController.LoadLevel(SceneController.SCENE_1);
        }
    }
}