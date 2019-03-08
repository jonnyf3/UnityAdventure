using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
using RPG.States;

namespace RPG.Characters
{
    public abstract class AICharacter : Character
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

        public PatrolPath PatrolPath => patrolPath;
        public float PatrolPathDelay => patrolWaypointDelay;
        public float PatrolPathTolerance => patrolWaypointTolerance;

        private Transform lookTarget = null;
        
        protected override void Awake() {
            base.Awake();

            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        protected override void Start() {
            base.Start();

            SetupNavMeshAgent();

            SetState<IdleState>();
        }

        protected virtual void Update() {
            Move();
            DetermineState();
        }

        protected void Move() {
            //Process any required movement via the CharacterMovement component
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (!arrivedAtTarget) {
                movement.Move(agent.desiredVelocity.normalized);
            } else if (lookTarget) {
                movement.TurnTowards(lookTarget);
            } else {
                movement.Move(Vector3.zero);
            }
            //Stop navmesh agent running away
            agent.velocity = Vector3.zero;
        }

        protected abstract void DetermineState();

        public void SetMoveTarget(Vector3 destination) {
            agent.SetDestination(destination);
        }
        public void StopMoving() {
            agent.SetDestination(transform.position);
        }
        public void SetLookTarget(Transform target) {
            lookTarget = target;
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
    }
}