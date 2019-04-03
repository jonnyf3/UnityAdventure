using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Movement
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float waypointPause = 3f;

        private void Start() {
            Assert.IsNotNull(patrolPath, "Cannot move platform without a patrol path");
            transform.position = patrolPath.Waypoint(0).position;
            StartCoroutine(Move());
        }

        #region Movement
        private Transform lastWaypoint, nextWaypoint;
        private Vector3 movePath => nextWaypoint.position - lastWaypoint.position;
        private IEnumerator Move() {
            lastWaypoint = patrolPath.Waypoint(0);
            nextWaypoint = patrolPath.Waypoint(1);
            while (true) {
                var moveTime = movePath.magnitude / moveSpeed;
                float t = 0;
                while (transform.position != nextWaypoint.position) {
                    transform.position = Vector3.Lerp(lastWaypoint.position, nextWaypoint.position, t/moveTime);
                    t += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(waypointPause);
                lastWaypoint = nextWaypoint;
                nextWaypoint = SetNextWaypoint(nextWaypoint);
            }
        }

        private int indexStep = 1;
        private Transform SetNextWaypoint(Transform currentWaypoint) {
            var currentWaypointIndex = currentWaypoint.GetSiblingIndex();
            int nextIndex;

            if (patrolPath.isLoop) {
                nextIndex = (currentWaypointIndex + indexStep) % patrolPath.WaypointCount;
            } else {
                if (currentWaypointIndex + indexStep < 0) {
                    indexStep = 1;
                }
                else if (currentWaypointIndex + indexStep >= patrolPath.WaypointCount) {
                    indexStep = -1;
                }
                nextIndex = currentWaypointIndex + indexStep;
            }

            return patrolPath.Waypoint(nextIndex);
        }
        #endregion

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.GetComponent<CharacterMovement>()) {
                other.transform.parent = transform;
            }
        }
        private void OnTriggerExit(Collider other) {
            other.transform.parent = null;
        }
    }
}