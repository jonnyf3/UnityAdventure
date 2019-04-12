using System;
using System.IO;
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
        private Scene mainScene;
        private Scene currentScene;

        private Fader fader;

        void Start() {
            fader = GetComponentInChildren<Fader>();
            mainScene = SceneManager.GetSceneByBuildIndex(0);
            currentScene = SceneManager.GetActiveScene();

            if (SceneManager.sceneCount == 1) {
                LoadLevel(MAIN_MENU);
            } else {
                fader.gameObject.SetActive(false);
            }
        }

        public void LoadLevel(string name) {
            Load(name);
        }
        public void LoadLevel(int index) {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(index);
            var sceneName = Path.GetFileNameWithoutExtension(scenePath);
            Load(sceneName);
        }
        public void ReloadLevel() {
            Load(currentScene.name);
        }

        private void Load(string sceneName) {
            StopAllCoroutines();
            StartCoroutine(LoadScene(sceneName));
        }
        private IEnumerator LoadScene(string sceneName) {
            fader.gameObject.SetActive(true);
            yield return fader.FadeOut(2.5f);

            SceneManager.SetActiveScene(mainScene);

            if (currentScene != mainScene) {
                yield return SceneManager.UnloadSceneAsync(currentScene);
            }
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            onLevelLoaded?.Invoke();

            currentScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(currentScene);

            yield return fader.FadeIn(1.5f);
            fader.gameObject.SetActive(false);
        }
    }
}