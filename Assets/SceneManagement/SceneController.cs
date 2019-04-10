using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        public const string MAIN_MENU = "MainMenu";
        public const string SCENE_1   = "Level";
        public const string SCENE_2   = "Dash Level";
        public const string SANDBOX   = "Sandbox";

        public event Action onLevelLoaded;
        private Scene currentScene;

        private Fader fader;

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

        public void LoadLevel(int index)   => Load(SceneManager.GetSceneByBuildIndex(index).name);
        public void LoadLevel(string name) => Load(name);
        public void ReloadLevel()          => Load(currentScene.name);

        private void Load(string sceneName) {
            StopAllCoroutines();
            StartCoroutine(LoadScene(sceneName));
        }
        private IEnumerator LoadScene(string sceneName) {
            //if (currentScene.name == sceneName) { yield break; }

            fader.gameObject.SetActive(true);
            yield return fader.FadeOut(1f);

            var load = SceneManager.UnloadSceneAsync(currentScene);
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            yield return load;
            onLevelLoaded?.Invoke();

            currentScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(currentScene);

            yield return fader.FadeIn(1.5f);
            fader.gameObject.SetActive(false);
        }
    }
}