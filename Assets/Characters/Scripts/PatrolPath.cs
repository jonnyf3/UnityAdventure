using UnityEngine;

namespace RPG.Characters
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos() {
            Vector3 firstPosition = transform.GetChild(0).position;
            Vector3 previousPosition = firstPosition;

            foreach (Transform waypoint in transform) {
                Gizmos.DrawLine(waypoint.position, previousPosition);
                previousPosition = waypoint.position;
                Gizmos.DrawWireSphere(waypoint.position, 0.5f);
            }
            Gizmos.DrawLine(previousPosition, firstPosition);
        }
    }
}