using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : Character
    {
        private NavMeshAgent agent;
        private Transform Target { get; set; }  //TODO allow for targets who are not the player
        [Header("NavMesh")]
        [SerializeField] float radius = 0.3f;
        [SerializeField] float height = 1.7f;
        [SerializeField] float speed = 1f;
        [SerializeField] float acceleration = 100f;
        [SerializeField] float stoppingDistance = 1.2f;

        private WeaponSystem combat;
        private bool isAttacking = false;
        [Header("Combat")]
        [SerializeField] float attackRadius = 10f;
        [SerializeField] float attacksPerSecond = 0.5f;
        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float turnSpeed = 2f;
        
        private Player player;

        protected override void Awake() {
            base.Awake();

            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        protected override void Start() {
            base.Start();

            combat = GetComponent<WeaponSystem>();
            SetupNavMeshAgent();

            player = FindObjectOfType<Player>();
            Assert.IsNotNull(player, "Could not find player in the scene!");
            player.onPlayerDied += OnPlayerDied;
        }

        void Update() {
            if (health.IsDead) { return; }

            if (IsPlayerInAttackRange()) {
                LookTowardsPlayer();

                //Attack only when looking (roughly) towards the player
                Vector3 unitVectorToTarget = (player.transform.position - transform.position).normalized;
                float angleTowardsPlayer = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
                if (angleTowardsPlayer < 10f) {
                    if (!isAttacking) { StartCoroutine(Attack()); }
                }
            }
            else {
                isAttacking = false;
                StopAllCoroutines();
            }
            
            var target = IsPlayerInChaseRange() ? player.transform.position : transform.position;
            agent.SetDestination(target);

            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (arrivedAtTarget) {
                movement.Move(Vector3.zero, false);
            }
            else {
                movement.Move(agent.desiredVelocity, false);
            }
        }

        private IEnumerator Attack() {
            isAttacking = true;
            while (true) {
                combat.Attack();
                yield return new WaitForSeconds(1f / attacksPerSecond);
            }
        }

        private float GetDistanceToPlayer() {
            if (player == null) return 1000f;

            return Vector3.Distance(player.transform.position, transform.position);
        }

        private bool IsPlayerInAttackRange() {
            //TODO expose combat.CurrentWeapon and get a max range from this?
            return GetDistanceToPlayer() <= attackRadius;
        }
        private bool IsPlayerInChaseRange() {
            return GetDistanceToPlayer() <= chaseRadius;
        }

        private void LookTowardsPlayer() {
            Vector3 vectorToPlayer = player.transform.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToPlayer, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(player.transform.position - transform.position);
        }

        protected override void Die() {
            //combat.EndAttack();
            Destroy(gameObject, 3f);
        }

        private void OnPlayerDied() {
            //Disable all features which require a player
            agent.SetDestination(transform.position);
            player = null;
        }

        //private void OnDrawGizmos() {
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(transform.position, chaseRadius);

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, attackRadius);
        //}

        private void SetupNavMeshAgent() {
            agent.radius = radius;
            agent.height = height;
            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
            agent.updatePosition = true;
        }
    }
}