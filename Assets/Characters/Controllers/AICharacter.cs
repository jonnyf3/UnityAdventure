using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
using RPG.States;

namespace RPG.Characters
{
    public abstract class AICharacter : Character
    {
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

        public PatrolPath PatrolPath => patrolPath;
        public float PatrolPathDelay => patrolWaypointDelay;
        public float PatrolPathTolerance => patrolWaypointTolerance;

        public float TurnSpeed => turnSpeed;

        protected override void Awake() {
            base.Awake();

            SetupNavMeshAgent();
        }

        protected override void Start() {
            base.Start();

            SetState<IdleState>();
        }

        private void SetupNavMeshAgent() {
            var agent = gameObject.AddComponent<NavMeshAgent>();

            agent.radius = radius;
            agent.height = height;
            agent.speed = 1;
            agent.acceleration = acceleration;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.isStopped = true;
        }
    }
}