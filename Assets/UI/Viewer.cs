using System;
using UnityEngine;
using RPG.Characters;

namespace RPG.UI
{
    public class Viewer : MonoBehaviour
    {
        public Vector3 LookTarget => GetRaycastTargetPoint();

        [SerializeField] Transform reticule = null;
        [SerializeField] float maxLookDistance = 50f;

        GameObject currentViewTarget;
        public event Action<GameObject> onViewTargetChanged;

        private void Start() { onViewTargetChanged?.Invoke(null); }

        private void Update() {
            var gameObjectHit = GetRaycastTargetObject();
            if (gameObjectHit != currentViewTarget) {
                currentViewTarget = gameObjectHit;
                onViewTargetChanged?.Invoke(currentViewTarget);
            }
        }

        private GameObject GetRaycastTargetObject() {
            //Do raycast through viewpoint, hitting only colliders and excluding player layer
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var raycastingMask = ~LayerMask.GetMask("Player");
            var hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxLookDistance, raycastingMask, QueryTriggerInteraction.Ignore);

            return hit ? hitInfo.collider.gameObject : null;
        }

        private Vector3 GetRaycastTargetPoint() {
            Ray ray = Camera.main.ScreenPointToRay(reticule.position);
            var raycastingMask = ~LayerMask.GetMask("Player");
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxLookDistance, raycastingMask, QueryTriggerInteraction.Ignore)) {
                return hitInfo.point;
            } else {
                return RayIntersectFromPlayer(ray, maxLookDistance);
            }
        }
        private Vector3 RayIntersectFromPlayer(Ray ray, float R) {
            //Calculate the point along the ray (from the camera through the viewpoint) which is at distance R from the player
            var cameraPos = Camera.main.transform.position;
            var camToPlayer = FindObjectOfType<Player>().transform.position - cameraPos;
            var theta = Vector3.SignedAngle(ray.direction, camToPlayer, Vector3.Cross(ray.direction, camToPlayer)) * Mathf.Deg2Rad;
            var d = camToPlayer.magnitude;
            var r = d * Mathf.Sin(theta);   //perpendicular distance from the player to the ray
            var distanceAlongRay = d * Mathf.Cos(theta) + Mathf.Sqrt(R*R - r*r);
            var rayEnd = cameraPos + (ray.direction * distanceAlongRay);
            return rayEnd;
        }
    }
}