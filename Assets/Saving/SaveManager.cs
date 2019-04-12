using System.IO;
using System.Text;
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
            using (FileStream file = File.Open(SaveFile, FileMode.Create)) {
                var byteString = Encoding.UTF8.GetBytes("Hello world!");
                file.Write(byteString, 0, byteString.Length);
            }
        }

        public void Load() {
            print("Loading game from " + SaveFile);
            using (FileStream file = File.Open(SaveFile, FileMode.Open)) {
                var buffer = new byte[file.Length];
                file.Read(buffer, 0, buffer.Length);
                print(Encoding.UTF8.GetString(buffer));
            }
        }
    }
}