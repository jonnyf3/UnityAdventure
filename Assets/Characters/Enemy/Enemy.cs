using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(EnemyCombat))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 10f;
        [SerializeField] float turnSpeed = 2f;
        
        private AICharacterMovement movement = null;
        private EnemyCombat combat = null;
        private Health health;
        private GameObject player = null;
        
        // Start is called before the first frame update
        void Start() {
            movement = GetComponent<AICharacterMovement>();
            combat = GetComponent<EnemyCombat>();

            health = GetComponent<Health>();
            health.onDeath += OnDeath;

            player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(player, "Could not find player in the scene!");
            player.GetComponent<Player>().onPlayerDied += OnPlayerDied;
        }

        private void LateUpdate() {
            //transform.position = movement.transform.position;
            //movement.transform.localPosition = Vector3.zero;
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
                movement.Target = player.transform;
            }
            else { movement.Target = transform; }
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
            movement.Target = transform;
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