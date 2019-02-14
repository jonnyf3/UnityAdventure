using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class AICharacter : Character
    {
        protected NavMeshAgent agent;
        [Header("NavMesh")]
        [SerializeField] float radius = 0.3f;
        [SerializeField] float height = 1.7f;
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float acceleration = 100f;

        [Header("Patrolling")]
        [SerializeField] protected PatrolPath patrolPath = null;
        [SerializeField] float patrolWaypointDelay = 0.5f;
        [SerializeField] float patrolWaypointTolerance = 1.5f;

        protected CharacterUI ui;
        private Viewer viewer;

        protected override void Awake() {
            base.Awake();

            agent = gameObject.GetComponentInChildren<NavMeshAgent>();
        }

        protected override void Start() {
            base.Start();

            SetupNavMeshAgent();

            ui = GetComponentInChildren<CharacterUI>();
            DeactivateUI();
            viewer = Camera.main.GetComponent<Viewer>();
            viewer.onChangedFocus += DeactivateUI;
        }

        protected virtual void Update() {
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (arrivedAtTarget) {
                movement.Move(Vector3.zero, false);
            }
            else {
                movement.Move(agent.desiredVelocity, false);
            }
            //Stop navmesh agent running away
            agent.transform.localPosition = Vector3.zero;
        }

        protected void SetMoveTarget(Vector3 destination) {
            agent.SetDestination(destination);
        }
        protected void StopMoving() {
            agent.SetDestination(transform.position);
        }

        protected IEnumerator Patrol() {
            Transform nextWaypoint = GetClosestWaypoint();
            while (true) {
                //Only set destination once - assumes waypoints do not move
                SetMoveTarget(nextWaypoint.position);
                while (!ArrivedAtWaypoint(nextWaypoint)) {
                    yield return new WaitForEndOfFrame();
                }
                StopMoving();
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

        private void SetupNavMeshAgent() {
            agent.radius = radius;
            agent.height = height;
            agent.speed = 1;
            agent.acceleration = acceleration;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
            agent.updatePosition = true;
        }

        public virtual void ActivateUI() {
            ui.gameObject.SetActive(true);
        }
        protected virtual void DeactivateUI() {
            ui.gameObject.SetActive(false);
        }

        private void OnDestroy() {
            viewer.onChangedFocus -= DeactivateUI;
        }
    }
}