using System.Collections;
using UnityEngine;
using RPG.Characters;
using RPG.Movement;

namespace RPG.States
{
    public class PatrollingState : AIState
    {
        private PatrolPath patrolPath         => (character as AICharacter).PatrolPath;
        private float patrolWaypointDelay     => (character as AICharacter).PatrolPathDelay;
        private float patrolWaypointTolerance => (character as AICharacter).PatrolPathTolerance;

        private float baseAnimatorForwardCap;

        public override void OnStateEnter() {
            base.OnStateEnter();

            //Should only ever walk while patrolling
            var movement = character.GetComponent<CharacterMovement>();
            baseAnimatorForwardCap = movement.AnimatorForwardCap;
            movement.AnimatorForwardCap = 0.5f;

            StartCoroutine(Patrol());
        }

        protected IEnumerator Patrol() {
            var nextWaypoint = GetClosestWaypoint();
            while (true) {
                //Only set destination once - assumes waypoints do not move
                while (!ArrivedAtWaypoint(nextWaypoint)) {
                    MoveTowards(nextWaypoint.position);
                    yield return new WaitForEndOfFrame();
                }
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
            StopAllCoroutines();
            character.GetComponent<CharacterMovement>().AnimatorForwardCap = baseAnimatorForwardCap;
        }
    }
}