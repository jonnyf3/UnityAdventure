using System.Collections;
using UnityEngine;
using RPG.Characters;
using RPG.Movement;

namespace RPG.States
{
    public class PatrollingState : State
    {
        private AICharacter ai;

        private PatrolPath patrolPath;
        private float patrolWaypointDelay = 0;
        private float patrolWaypointTolerance = 0;
        private float baseAnimatorForwardCap = 1f;
        
        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            ai = character as AICharacter;
            character.GetComponent<CharacterMovement>().AnimatorForwardCap = 0.5f;

            StartCoroutine(Patrol());
        }

        public override void SetArgs(StateArgs args)     {
            base.SetArgs(args);

            var idleArgs = args as PatrollingStateArgs;
            this.patrolPath = idleArgs.path;
            this.patrolWaypointDelay = idleArgs.patrolWaypointDelay;
            this.patrolWaypointTolerance = idleArgs.patrolWaypointTolerance;
            this.baseAnimatorForwardCap = idleArgs.animatorForwardCap;
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

        public override void OnStateExit() {
            StopAllCoroutines();
            ai.StopMoving();
            character.GetComponent<CharacterMovement>().AnimatorForwardCap = baseAnimatorForwardCap;
        }
    }

    public class PatrollingStateArgs : StateArgs
    {
        public PatrolPath path;
        public float patrolWaypointDelay;
        public float patrolWaypointTolerance;
        public float animatorForwardCap;

        public PatrollingStateArgs(AICharacter character, PatrolPath patrolPath, float patrolWaypointDelay, float patrolWaypointTolerance, float animatorForwardCap) : base(character)
        {
            this.path = patrolPath;
            this.patrolWaypointDelay = patrolWaypointDelay;
            this.patrolWaypointTolerance = patrolWaypointTolerance;
            this.animatorForwardCap = animatorForwardCap;
        }
    }
}