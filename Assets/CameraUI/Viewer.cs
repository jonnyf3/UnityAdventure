using UnityEngine;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] Transform reticule = null;
        private GameObject currentViewTarget = null;

        float maxRaycastDepth = 50f;

        public delegate void OnChangedFocus();
        public event OnChangedFocus onChangedFocus;
        
        // Update is called once per frame
        void Update() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var gameObjectHit = DoRaycast(ray);

            if (gameObjectHit != currentViewTarget) {
                onChangedFocus();
                currentViewTarget = gameObjectHit;
                NotifyNewFocusTarget();
            }
        }

        private GameObject DoRaycast(Ray ray) {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            RaycastHit hitInfo;
            LayerMask mask = ~LayerMask.GetMask("Player");
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth, mask, QueryTriggerInteraction.Ignore);

            return hitInfo.collider.gameObject;
        }

        private void NotifyNewFocusTarget() {
            var npcHit = currentViewTarget.GetComponent<NPCController>();
            if (npcHit) {
                npcHit.ActivateUI();
            }
        }
    }
}