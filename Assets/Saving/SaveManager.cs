using System;
using System.IO;
using UnityEngine;
using RPG.Characters;

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
                var byteString = SerializeVector(FindObjectOfType<Player>().transform.position);
                file.Write(byteString, 0, byteString.Length);
            }
        }
        public void Load() {
            print("Loading game from " + SaveFile);
            using (FileStream file = File.Open(SaveFile, FileMode.Open)) {
                var buffer = new byte[file.Length];
                file.Read(buffer, 0, buffer.Length);
                FindObjectOfType<Player>().transform.position = DeserializeVector(buffer);
            }
        }
        
        private byte[] SerializeVector(Vector3 vector) {
            byte[] vectorBuffer = new byte[3 * 4];
            BitConverter.GetBytes(vector.x).CopyTo(vectorBuffer, 0);
            BitConverter.GetBytes(vector.y).CopyTo(vectorBuffer, 4);
            BitConverter.GetBytes(vector.z).CopyTo(vectorBuffer, 8);
            return vectorBuffer;
        }
        private Vector3 DeserializeVector(byte[] buffer) {
            float x = BitConverter.ToSingle(buffer, 0);
            float y = BitConverter.ToSingle(buffer, 4);
            float z = BitConverter.ToSingle(buffer, 8);

            return new Vector3(x, y, z);
        }
    }
}