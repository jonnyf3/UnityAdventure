using UnityEngine;

namespace RPG.Movement
{
    public class PatrolPath : MonoBehaviour
    {
        public bool isLoop = true;

        public Transform Waypoint(int index) => transform.GetChild(index);
        public int WaypointCount => transform.childCount;

        private void OnDrawGizmos() {
            Vector3 firstPosition = Waypoint(0).position;
            Vector3 previousPosition = firstPosition;

            foreach (Transform waypoint in transform) {
                Gizmos.DrawLine(waypoint.position, previousPosition);
                previousPosition = waypoint.position;
                Gizmos.DrawWireSphere(waypoint.position, 0.5f);
            }

            if (isLoop) {
                Gizmos.DrawLine(previousPosition, firstPosition);
            }
        }
    }
}