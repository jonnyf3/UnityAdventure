using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;
using RPG.States;

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
        [SerializeField] protected float patrolWaypointDelay = 0.5f;
        [SerializeField] protected float patrolWaypointTolerance = 1.5f;
        [SerializeField] float turnSpeed = 2f;  //TODO this isn't really to do with patrolling?

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

            var idleArgs = new IdlingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
            SetState<IdlingState>(idleArgs);
        }

        protected virtual void Update() {
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (arrivedAtTarget) {
                movement.Move(Vector3.zero, false, false);
            }
            else {
                movement.Move(agent.desiredVelocity, false, focussed);
            }
            //Stop navmesh agent running away
            agent.transform.localPosition = Vector3.zero;
        }

        public void SetMoveTarget(Vector3 destination) {
            agent.SetDestination(destination);
        }
        public void StopMoving() {
            if (!agent) { return; }
            agent.SetDestination(transform.position);
        }

        public void TurnTowardsTarget(Transform target) {
            Vector3 vectorToTarget = target.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToTarget, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(target.position - transform.position);
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