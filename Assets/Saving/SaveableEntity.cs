using System.Collections.Generic;
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
            var entityState = new Dictionary<string, object>();
            foreach(ISaveable saveable in GetComponents<ISaveable>()) {
                var component = saveable.GetType().ToString();
                entityState[component] = saveable.SaveState();
            }
            return entityState;
        }
        public void LoadState(object state) {
            var entityState = (Dictionary<string, object>)state;
            foreach (ISaveable saveableComponent in GetComponents<ISaveable>()) {
                var component = saveableComponent.GetType().ToString();
                if (entityState.ContainsKey(component)) {
                    saveableComponent.LoadState(entityState[component]);
                } else {
                    Destroy(saveableComponent as Component);
                }
            }
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