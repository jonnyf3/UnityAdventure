using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(SaveFile, FileMode.Create)) {
                Vector3 playerPosition = FindObjectOfType<Player>().transform.position;
                bf.Serialize(file, new SerializableVector3(playerPosition));
            }
        }

        public void Load() {
            print("Loading game from " + SaveFile);
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(SaveFile, FileMode.Open)) {
                SerializableVector3 vec = (SerializableVector3)bf.Deserialize(file);
                FindObjectOfType<Player>().transform.position = vec.ToVector();
            }
        }
    }


    [Serializable]
    public class SerializableVector3
    {
        private float x, y, z;

        public SerializableVector3(Vector3 vector) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public Vector3 ToVector() { return new Vector3(x, y, z); }
    }
}