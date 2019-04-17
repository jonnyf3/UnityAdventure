using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RPG.Saving
{
    public class SaveManager : MonoBehaviour
    {
        private string saveFile = "save";
        private string SaveFile {
            get { return Path.Combine(Application.persistentDataPath, saveFile + ".sav"); }
        }

        public void Save() {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(SaveFile, FileMode.Create)) {
                bf.Serialize(file, GetGameState());
            }
            print("Game saved!");
        }
        private object GetGameState() {
            var state = new Dictionary<string, object>();
            foreach (var entity in FindObjectsOfType<SaveableEntity>()) {
                state[entity.GUID] = entity.SaveState();
            }
            return state;
        }

        public void Load() {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(SaveFile, FileMode.Open)) {
                RestoreState(bf.Deserialize(file));
            }
            print("Game loaded!");
        }
        private void RestoreState(object s) {
            var state = (Dictionary<string, object>)s;
            foreach (var entity in FindObjectsOfType<SaveableEntity>()) {
                if (state.ContainsKey(entity.GUID)) {
                    entity.LoadState(state[entity.GUID]);
                } else {
                    Destroy(entity.gameObject);
                }
            }
        }
    }
}