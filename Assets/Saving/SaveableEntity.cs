using UnityEngine;

namespace RPG.Saving
{
    class SaveableEntity : MonoBehaviour
    {
        public string GUID {
            get { return ""; }
        }

        public object SaveState() {
            print("Saving state for " + gameObject.name + " (" + GUID + ")");
            return null;
        }

        public void LoadState(object state) {
            print("Loading state for " + gameObject.name + " (" + GUID + ")");
        }
    }
}