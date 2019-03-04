using UnityEngine;

namespace RPG.UI
{
    public class Viewer : MonoBehaviour
    {
        public Vector3 LookTarget => GetRaycastTargetPoint();

        [SerializeField] Transform reticule = null;
        [SerializeField] float maxLookDistance = 50f;
        GameObject currentViewTarget;
        
        void Update() {
            var gameObjectHit = GetRaycastTargetObject();

            if (gameObjectHit != currentViewTarget) {
                currentViewTarget = gameObjectHit;

                DisableAllUI();
                ShowTargetObjectUI();
            }
        }

        private GameObject GetRaycastTargetObject() {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var raycastingMask = ~LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxLookDistance, raycastingMask, QueryTriggerInteraction.Ignore);

            //If no hit (e.g. looking at skybox), return camera as current target
            return hit ? hitInfo.collider.gameObject : gameObject;
        }

        private Vector3 GetRaycastTargetPoint() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var raycastingMask = ~LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxLookDistance, raycastingMask, QueryTriggerInteraction.Ignore);

            if (hit) { return hitInfo.point; }
            else { return transform.position + (ray.direction * maxLookDistance); }
        }

        private void DisableAllUI() {
            foreach (var ai in FindObjectsOfType<CharacterUI>()) {
                ai.Show(false);
            }
        }
        private void ShowTargetObjectUI() {
            if (currentViewTarget.GetComponentInChildren<CharacterUI>()) {
                currentViewTarget.GetComponentInChildren<CharacterUI>().Show(true);
            }
        }
    }
}