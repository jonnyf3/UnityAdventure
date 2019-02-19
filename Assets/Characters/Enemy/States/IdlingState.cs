using System.Collections;
using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public class IdlingState : State
    {
        private PatrolPath patrolPath;
        private float patrolWaypointDelay = 0;
        private float patrolWaypointTolerance = 0;
        
        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            var idleArgs = args as IdlingStateArgs;
            this.patrolPath = idleArgs.path;
            this.patrolWaypointDelay = idleArgs.patrolWaypointDelay;
            this.patrolWaypointTolerance = idleArgs.patrolWaypointTolerance;

            character.GetComponent<CharacterMovement>().AnimatorForwardCap = 0.5f;

            if (patrolPath) {
                StartCoroutine(Patrol());
            }
        }

        protected IEnumerator Patrol() {
            Transform nextWaypoint = GetClosestWaypoint();
            while (true) {
                //Only set destination once - assumes waypoints do not move
                character.SetMoveTarget(nextWaypoint.position);
                while (!ArrivedAtWaypoint(nextWaypoint)) {
                    yield return new WaitForEndOfFrame();
                }
                character.StopMoving();
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

        public void OnDestroy() {
            StopAllCoroutines();
            character.GetComponent<CharacterMovement>().AnimatorForwardCap = 1f;
            character.StopMoving();
        }
    }

    public class IdlingStateArgs : StateArgs
    {
        public PatrolPath path;
        public float patrolWaypointDelay;
        public float patrolWaypointTolerance;

        public IdlingStateArgs(AICharacter character, PatrolPath patrolPath, float patrolWaypointDelay, float patrolWaypointTolerance) : base(character)
        {
            this.path = patrolPath;
            this.patrolWaypointDelay = patrolWaypointDelay;
            this.patrolWaypointTolerance = patrolWaypointTolerance;
        }
    }
}