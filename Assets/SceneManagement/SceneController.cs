using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        private static SceneController instance;
        private static string currentScene;
        private void Awake() { instance = this; }

        private const string MAIN_MENU = "MainMenu";
        public const string SCENE_1 = "Level";
        public const string SCENE_2 = "Dash Level";
        public const string SANDBOX = "Sandbox";

        void Start() {
            LoadLevel(MAIN_MENU);
        }

        public static void LoadLevel(string sceneName) {
            instance.StartCoroutine(instance.LoadScene(sceneName));
        }

        private IEnumerator LoadScene(string sceneName) {
            yield return Load(sceneName);
            var newScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(newScene);

            yield return UnloadCurrent();
            currentScene = sceneName;
        }

        private AsyncOperation Load(string sceneName) {
            if (SceneManager.GetSceneByName(sceneName).isLoaded) { return null; }

            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        private AsyncOperation UnloadCurrent() {
            if (currentScene == null) { return null; }

            return SceneManager.UnloadSceneAsync(currentScene);
        }
    }
}