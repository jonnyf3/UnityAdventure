using System.IO;
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
            print("Saving game to " + SaveFile);
        }

        public void Load() {
            print("Loading game from " + SaveFile);
        }
    }
}