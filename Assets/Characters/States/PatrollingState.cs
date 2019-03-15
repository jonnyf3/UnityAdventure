using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Movement;

namespace RPG.States
{
    public class PatrollingState : IdleState
    {
        private AICharacter ai;

        private PatrolPath patrolPath => ai.PatrolPath;
        private float patrolWaypointDelay => ai.PatrolPathDelay;
        private float patrolWaypointTolerance => ai.PatrolPathTolerance;

        private int indexStep = 1;
        
        protected override void Start() {
            base.Start();
            ai = character as AICharacter;
            Assert.IsNotNull(ai, "PatrollingState should only be entered by an AI character");

            GetComponent<CharacterMovement>().SetAnimatorForwardCap(0.5f);

            StartCoroutine(Patrol());
        }

        protected IEnumerator Patrol() {
            var nextWaypoint = GetClosestWaypoint();
            while (true) {
                //Only set destination once - assumes waypoints do not move
                ai.SetMoveTarget(nextWaypoint.position);
                while (!ArrivedAtWaypoint(nextWaypoint)) {
                    yield return new WaitForEndOfFrame();
                }
                ai.StopMoving();
                yield return new WaitForSeconds(patrolWaypointDelay);
                nextWaypoint = SetNextWaypoint(nextWaypoint);
            }
        }

        private bool ArrivedAtWaypoint(Transform waypoint) {
            return Vector3.Distance(transform.position, waypoint.position) <= patrolWaypointTolerance;
        }

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

        private Transform GetClosestWaypoint() {
            Transform closestWaypoint = null;
            float shortestDistance = 1000f;   //large number which should be immediately overwritten
            foreach (Transform waypoint in patrolPath.transform) {
                var distanceToWaypoint = Vector3.Distance(transform.position, waypoint.position);
                if (distanceToWaypoint < shortestDistance) {
                    shortestDistance = distanceToWaypoint;
                    closestWaypoint = waypoint;
                }
            }
            return closestWaypoint;
        }

        private void OnDestroy() {
            GetComponent<CharacterMovement>().ResetAnimatorForwardCap();
        }
    }
}