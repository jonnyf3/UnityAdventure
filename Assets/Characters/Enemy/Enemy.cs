using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(EnemyCombat))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 10f;
        [SerializeField] float turnSpeed = 2f;
        
        private AICharacterControl ai = null;
        private EnemyCombat combat = null;
        private Health health = null;
        private GameObject player = null;

        private bool isDead = false;

        // Start is called before the first frame update
        void Start() {
            ai = GetComponentInChildren<AICharacterControl>();
            combat = GetComponent<EnemyCombat>();

            health = GetComponent<Health>();
            health.onDeath += OnDeath;

            player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(player, "Could not find player in the scene!");
            player.GetComponent<Player>().onPlayerDied += OnPlayerDied;
        }

        private void LateUpdate() {
            transform.position = ai.transform.position;
            ai.transform.localPosition = Vector3.zero;
        }

        // Update is called once per frame
        void Update() {
            if (isDead) { return; }

            if (IsPlayerInAttackRange()) {
                LookTowardsPlayer();
                combat.Attack(player);
            }
            else { combat.EndAttack(); }

            if (IsPlayerInChaseRange()) {
                ai.SetTarget(player.transform);
            }
            else { ai.SetTarget(ai.transform); }
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
            var animator = GetComponentInChildren<Animator>();
            animator.SetBool("isDead", true);

            isDead = true;
            combat.EndAttack();
            Destroy(gameObject, 3f);
        }

        private void OnPlayerDied() {
            //Disable all features which require a player
            ai.SetTarget(ai.transform);
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