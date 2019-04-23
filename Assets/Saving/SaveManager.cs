using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using RPG.SceneManagement;

namespace RPG.Saving
{
    public class SaveManager : MonoBehaviour
    {
        private string saveFile = "save";
        private string SaveFile {
            get { return Path.Combine(Application.persistentDataPath, saveFile + ".sav"); }
        }

        public void Save() {
            var sc = FindObjectOfType<SceneController>();
            var state = CaptureState(LoadFromFile(), sc.CurrentScene);
            SaveToFile(state);
            print("Game saved!");
        }
        private object CaptureState(Dictionary<string, object> gameState, string currentScene) {
            var sceneState = new Dictionary<string, object>();
            foreach (var entity in FindObjectsOfType<SaveableEntity>()) {
                sceneState[entity.GUID] = entity.SaveState();
            }
            gameState[currentScene] = sceneState;
            return gameState;
        }

        public void Load() {
            var gameState = LoadFromFile();
            var sc = FindObjectOfType<SceneController>();
            if (gameState.ContainsKey(sc.CurrentScene)) {
                var sceneState = (Dictionary<string, object>)gameState[sc.CurrentScene];
                RestoreState(sceneState);
            }
            print("Game loaded!");
        }
        private void RestoreState(Dictionary<string, object> state) {
            foreach (var entity in FindObjectsOfType<SaveableEntity>()) {
                if (state.ContainsKey(entity.GUID)) {
                    entity.LoadState(state[entity.GUID]);
                } else {
                    Destroy(entity.gameObject);
                }
            }
        }

        #region FileIO
        private Dictionary<string, object> LoadFromFile() {
            if (!File.Exists(SaveFile)) { return new Dictionary<string, object>(); }

            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(SaveFile, FileMode.Open)) {
                return (Dictionary<string, object>)bf.Deserialize(file);
            }
        }
        private void SaveToFile(object state) {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(SaveFile, FileMode.Create)) {
                bf.Serialize(file, state);
            }
        }
        #endregion
    }
}