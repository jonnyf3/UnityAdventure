using UnityEngine;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] Transform reticule;

        float maxRaycastDepth = 100f;

        public delegate void OnLookingAtNPC();
        public event OnLookingAtNPC onLookingAtNPC;

        // Update is called once per frame
        void Update() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            if (RaycastForCharacter(ray)) { return; }
        }

        private bool RaycastForCharacter(Ray ray) {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            RaycastHit hitInfo;
            LayerMask mask = ~LayerMask.GetMask("Player");
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth, mask, QueryTriggerInteraction.Ignore);

            var gameObjectHit = hitInfo.collider.gameObject;
            var npcHit = gameObjectHit.GetComponent<NPCController>();
            if (npcHit) {
                onLookingAtNPC();
                return true;
            }
            return false;
        }
    }
}