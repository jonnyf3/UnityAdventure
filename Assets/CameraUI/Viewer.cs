using UnityEngine;
using RPG.Characters;
using System;

namespace RPG.CameraUI
{
    public class Viewer : MonoBehaviour
    {
        public Vector3 LookTarget {
            get { return GetRaycastTargetPoint(); }
        }

        [SerializeField] Transform reticule = null;
        private GameObject currentViewTarget;

        float maxRaycastDepth = 50f;

        public delegate void OnChangedFocus();
        public event OnChangedFocus onChangedFocus;
        
        void Update() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var gameObjectHit = GetRaycastTarget(ray);

            if (gameObjectHit != currentViewTarget) {
                onChangedFocus?.Invoke();
                currentViewTarget = gameObjectHit;
            }

            NotifyFocusTarget();
        }

        private GameObject GetRaycastTarget(Ray ray) {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            RaycastHit hitInfo;
            LayerMask mask =~ LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, mask, QueryTriggerInteraction.Ignore);

            //If no hit (e.g. looking at skybox), return camera as current target
            return hit ? hitInfo.collider.gameObject : gameObject;
        }


        private Vector3 GetRaycastTargetPoint() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            RaycastHit hitInfo;
            LayerMask mask = ~LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, mask, QueryTriggerInteraction.Ignore);

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