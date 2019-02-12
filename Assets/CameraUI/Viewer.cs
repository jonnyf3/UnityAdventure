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
            var gameObjectHit = GetRaycastTarget(ray);

            if (gameObjectHit != currentViewTarget) {
                onChangedFocus();
                currentViewTarget = gameObjectHit;
                NotifyNewFocusTarget();
            }
        }

        private GameObject GetRaycastTarget(Ray ray) {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            RaycastHit hitInfo;
            LayerMask mask =~ LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, mask, QueryTriggerInteraction.Ignore);

            //If no hit (e.g. looking at skybox), return camera as current target
            return hit ? hitInfo.collider.gameObject : gameObject;
        }

        private void NotifyNewFocusTarget() {
            //TODO generalise to other targets (Character class?)
            var characterHit = currentViewTarget.GetComponent<Character>();
            if (characterHit) {
                characterHit.ActivateUI();
            }
        }
    }
}