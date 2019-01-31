using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.ThirdPerson;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] float attackRadius = 5f;
        [SerializeField] float shotsPerSecond = 1f;
        [SerializeField] float chaseRadius = 10f;
        [SerializeField] float turnSpeed = 2f;

        [SerializeField] Projectile projectile = null;
        [SerializeField] Transform projectileSocket = null;

        private AICharacterControl ai = null;
        private Health health = null;
        private GameObject player = null;
        private bool isAttacking = false;

        // Start is called before the first frame update
        void Start() {
            ai = GetComponentInChildren<AICharacterControl>();

            health = GetComponent<Health>();
            health.onDeath += Die;

            player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(player, "Could not find player in the scene!");
            player.GetComponent<Player>().onPlayerDied += OnPlayerDied;
        }

        private void LateUpdate() {
            transform.position += ai.transform.localPosition; ;
            ai.transform.localPosition = Vector3.zero;
        }

        // Update is called once per frame
        void Update() {
            if (!player) { return; }

            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            //begin attacking when in range
            if (distanceToPlayer <= attackRadius) {
                LookTowardsPlayer();
                if (!isAttacking) {
                    InvokeRepeating("FireProjectile", 0f, 1f / shotsPerSecond);
                    isAttacking = true;
                }
            }
            //stop attacking if out of range
            if (distanceToPlayer > attackRadius && isAttacking) {
                CancelInvoke();
                isAttacking = false;
            }

            if (distanceToPlayer <= chaseRadius) {
                ai.SetTarget(player.transform);
            }
            else ai.SetTarget(transform);
        }

        private void LookTowardsPlayer() {
            Vector3 vectorToPlayer = player.transform.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToPlayer, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(player.transform.position - transform.position);
        }

        private void FireProjectile() {
            //manual offset to aim at player body rather than feet
            //TODO implement this in a better way (empty "target" transform on player?)
            Vector3 playerPosition = player.transform.position + new Vector3(0, 1f, 0);
            Vector3 unitVectorToPlayer = (playerPosition - projectileSocket.transform.position).normalized;

            //Fire only when pointed (roughly) towards the player
            if (Vector3.Angle(unitVectorToPlayer, transform.forward) < 10.0)
            {
                Projectile newProjectile = Instantiate(projectile, projectileSocket.transform.position, Quaternion.identity);
                float projectileSpeed = newProjectile.Speed;
                newProjectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
            }
        }

        private void Die() {
            Destroy(gameObject);
        }

        private void OnPlayerDied() {
            //Disable all features which require a player
            ai.SetTarget(transform);
            CancelInvoke();
            isAttacking = false;
            player = null;

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