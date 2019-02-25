using UnityEngine;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class Viewer : MonoBehaviour
    {
        public Vector3 LookTarget {
            get { return GetRaycastTargetPoint(); }
        }

        [SerializeField] Transform reticule = null;
        GameObject currentViewTarget;
        float maxRaycastDepth = 50f;

        public delegate void OnChangedFocus();
        public event OnChangedFocus onChangedFocus;
        
        void Update() {
            var gameObjectHit = GetRaycastTarget();

            if (gameObjectHit != currentViewTarget) {
                onChangedFocus?.Invoke();
                currentViewTarget = gameObjectHit;
            }

            NotifyFocusTarget();
        }

        private GameObject GetRaycastTarget() {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var raycastingMask = ~LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxRaycastDepth, raycastingMask, QueryTriggerInteraction.Ignore);

            //If no hit (e.g. looking at skybox), return camera as current target
            return hit ? hitInfo.collider.gameObject : gameObject;
        }


        private Vector3 GetRaycastTargetPoint() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var raycastingMask = ~LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxRaycastDepth, raycastingMask, QueryTriggerInteraction.Ignore);

            if (hit) { return hitInfo.point; }
            else { return transform.position + (ray.direction * maxRaycastDepth); }
        }

        private void NotifyFocusTarget() {
            var characterHit = currentViewTarget.GetComponent<AICharacter>();
            if (characterHit) {
                characterHit.ActivateUI();
            }
        }
    }
}