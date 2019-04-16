using UnityEngine;
using UnityEditor;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string guid = "";
        public string GUID {
            get { return guid; }
        }

        public object SaveState() {
            print("Saving state for " + gameObject.name + " (" + GUID + ")");
            return null;
        }
        public void LoadState(object state) {
            print("Loading state for " + gameObject.name + " (" + GUID + ")");
        }

        private void Update() {
            bool isPrefab = string.IsNullOrEmpty(gameObject.scene.path);
            if (Application.IsPlaying(gameObject) || isPrefab) { return; }

            var so = new SerializedObject(this);
            var guidProperty = so.FindProperty("guid");

            if (string.IsNullOrEmpty(guidProperty.stringValue)) {
                guidProperty.stringValue = System.Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();
            }
        }
    }
}