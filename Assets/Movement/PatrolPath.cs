using UnityEngine;

namespace RPG.Movement
{
    public class PatrolPath : MonoBehaviour
    {
        public bool isLoop = true;
        private const float WAYPOINT_GIZMO_RADIUS = 0.2f;

        public Transform Waypoint(int index) => transform.GetChild(index);
        public int WaypointCount => transform.childCount;

        private void OnDrawGizmos() {
            Vector3 firstPosition = Waypoint(0).position;
            Vector3 previousPosition = firstPosition;

            foreach (Transform waypoint in transform) {
                Gizmos.DrawLine(waypoint.position, previousPosition);
                previousPosition = waypoint.position;
                Gizmos.DrawWireSphere(waypoint.position, WAYPOINT_GIZMO_RADIUS);
            }

            if (isLoop) {
                Gizmos.DrawLine(previousPosition, firstPosition);
            }
        }
    }
}