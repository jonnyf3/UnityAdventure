using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
using RPG.States;
using RPG.Saving;

namespace RPG.Characters
{
    public abstract class AICharacter : Character, ISaveable
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

        protected override void Awake() {
            base.Awake();
            
            SetupNavMeshAgent();
        }

        public override void Move(Vector3 destination, float maxForwardCap = 1f) {
            agent.SetDestination(destination);

            //Process any required movement via the CharacterMovement component
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (!arrivedAtTarget) {
                movement.Move(agent.desiredVelocity.normalized, maxForwardCap);
            } else {
                movement.Move(Vector3.zero, maxForwardCap);
            }
            //Stop navmesh agent running away
            agent.velocity = Vector3.zero;
        }

        public override void SetDefaultState() { SetState<IdleState>(); }
        
        private void SetupNavMeshAgent() {
            var startPos = transform.position;
            bool startOnGround = IsOnGround;

            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.radius = radius;
            agent.height = height;
            agent.speed = 1;
            agent.acceleration = acceleration;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
            agent.updatePosition = true;

            if (!startOnGround) {
                agent.enabled = false;
                transform.position = startPos;
            }
        }

        #region SaveLoad
        public object SaveState() {
            return new CharacterSaveData(transform.position, transform.eulerAngles);
        }

        public void LoadState(object state) {
            var charState = (CharacterSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = charState.position.ToVector();
            transform.eulerAngles = charState.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
        #endregion
    }
}