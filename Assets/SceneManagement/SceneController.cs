using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        private static SceneController instance;
        private Scene currentScene;

        private Fader fader;

        private const string MAIN_MENU = "MainMenu";
        public  const string SCENE_1   = "Level";
        public  const string SCENE_2   = "Dash Level";
        public  const string SANDBOX   = "Sandbox";

        public static event Action onLevelLoaded;

        private void Awake() {
            instance = this;
        }
        void Start() {
            fader = GetComponentInChildren<Fader>();
            if (SceneManager.sceneCount == 1) {
                currentScene = SceneManager.GetSceneByBuildIndex(0);
                LoadLevel(MAIN_MENU);
            } else {
                currentScene = SceneManager.GetActiveScene();
                fader.gameObject.SetActive(false);
            }
        }

        public static void LoadLevel(int index)   => instance.Load(SceneManager.GetSceneByBuildIndex(index).name);
        public static void LoadLevel(string name) => instance.Load(name);

        private void Load(string sceneName) {
            StopAllCoroutines();
            StartCoroutine(LoadScene(sceneName));
        }
        private IEnumerator LoadScene(string sceneName) {
            if (currentScene.name == sceneName) { yield break; }

            fader.gameObject.SetActive(true);
            yield return fader.FadeOut(1f);

            yield return SceneManager.UnloadSceneAsync(currentScene);

            if (!SceneManager.GetSceneByName(sceneName).isLoaded) {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            currentScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(currentScene);

            onLevelLoaded?.Invoke();

            yield return fader.FadeIn(1.5f);
            fader.gameObject.SetActive(false);
        }
    }
}