using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(EnemyCombat))]
    public class Enemy : Character
    {
        [SerializeField] float chaseRadius = 10f;
        [SerializeField] float turnSpeed = 2f;
        
        private EnemyCombat combat = null;
        private NavMeshAgent agent;

        public Transform Target { get; set; }

        private GameObject player = null;

        // Start is called before the first frame update
        protected override void Start() {
            base.Start();

            combat = GetComponent<EnemyCombat>();
            player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(player, "Could not find player in the scene!");

            health.onDeath += OnDeath;
            player.GetComponent<Player>().onPlayerDied += OnPlayerDied;

            //TODO create at run-time
            agent = GetComponent<NavMeshAgent>();
            Assert.IsNotNull(agent, "AI Characters must have NavMesh Agents on their Body");
            agent.updateRotation = true;
            agent.updatePosition = true;
        }

        // Update is called once per frame
        void Update() {
            if (health.IsDead) { return; }

            if (IsPlayerInAttackRange()) {
                LookTowardsPlayer();
                combat.Attack(player);
            }
            else { combat.EndAttack(); }

            if (IsPlayerInChaseRange()) {
                Target = player.transform;
            }
            else { Target = transform; }

            agent.SetDestination(Target.position);

            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (arrivedAtTarget) {
                movement.Move(Vector3.zero, false);
            }
            else {
                movement.Move(agent.desiredVelocity, false);
            }

            //Stop the AI main character body rotating - the superposition of base rotation + body rotation does weird things
            //TODO work out why this is happening?
            //transform.rotation = Quaternion.identity;
        }

        private float GetDistanceToPlayer() {
            if (player == null) return 1000f;

            return Vector3.Distance(player.transform.position, transform.position);
        }

        private bool IsPlayerInAttackRange() {
            return GetDistanceToPlayer() <= combat.AttackRadius;
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

        private void OnDeath() {
            combat.EndAttack();
            Destroy(gameObject, 3f);
        }

        private void OnPlayerDied() {
            //Disable all features which require a player
            Target = transform;
            //player = null;

            //GetComponentInChildren<EnemyUI>().enabled = false;
        }

        //private void OnDrawGizmos() {
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(transform.position, chaseRadius);

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, attackRadius);
        //}
    }
}