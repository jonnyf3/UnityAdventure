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

        private float baseAnimatorForwardCap;

        protected override void Start() {
            base.Start();
            ai = character as AICharacter;
            Assert.IsNotNull(ai, "PatrollingState should only be entered by an AI character");

            var movement = GetComponent<CharacterMovement>();
            baseAnimatorForwardCap = movement.AnimatorForwardCap;
            movement.AnimatorForwardCap = 0.5f;

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
                int nextIndex = (nextWaypoint.GetSiblingIndex() + 1) % patrolPath.transform.childCount;
                nextWaypoint = patrolPath.transform.GetChild(nextIndex);
            }
        }

        private bool ArrivedAtWaypoint(Transform waypoint) {
            return Vector3.Distance(transform.position, waypoint.position) <= patrolWaypointTolerance;
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
            character.GetComponent<CharacterMovement>().AnimatorForwardCap = baseAnimatorForwardCap;
        }
    }
}