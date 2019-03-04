using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
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

        protected override void Awake() {
            base.Awake();

            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        protected override void Start() {
            base.Start();

            SetupNavMeshAgent();

            DoPassiveBehaviour();
        }

        protected void Move() {
            //Process any required movement via the CharacterMovement component
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            Vector3 moveVector = arrivedAtTarget ? Vector3.zero : agent.desiredVelocity.normalized;
            
            //Stop navmesh agent running away
            agent.velocity = Vector3.zero;

            movement.Move(moveVector);
        }

        public void SetMoveTarget(Vector3 destination) {
            agent.SetDestination(destination);
        }
        public void StopMoving() {
            agent.SetDestination(transform.position);
        }

        public void TurnTowardsTarget(Transform target) {
            Vector3 vectorToTarget = target.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToTarget, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(target.position - transform.position);
        }

        protected void DoPassiveBehaviour() {
            if (patrolPath) {
                var patrolArgs = new PatrollingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance, movement.AnimatorForwardCap);
                SetState<PatrollingState>(patrolArgs);
            }
            else {
                SetState<IdleState>(new StateArgs(this));
            }
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