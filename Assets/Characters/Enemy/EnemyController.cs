using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : AICharacter
    {
        private WeaponSystem combat;
        private bool isAttacking = false;
        [Header("Combat")]
        [SerializeField] float attackRadius = 10f;
        [SerializeField] float attacksPerSecond = 0.5f;
        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float turnSpeed = 2f;

        private Transform target;
        private Transform Target {
            get { return target; }
            set {
                if (target) { target.GetComponent<Character>().onDeath -= OnTargetDied; }
                target = value;
                if (target) { target.GetComponent<Character>().onDeath += OnTargetDied; }
            }
        }

        protected override void Start() {
            base.Start();

            combat = GetComponent<WeaponSystem>();
            
            var player = FindObjectOfType<Player>();
            Assert.IsNotNull(player, "Could not find player in the scene!");

            Target = player.transform;
        }

        void Update() {
            if (health.IsDead) { return; }

            if (IsTargetInAttackRange()) {
                LookTowardsTarget();

                //Attack only when looking (roughly) towards the player
                Vector3 unitVectorToTarget = (Target.position - transform.position).normalized;
                float angleTowardsTarget = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
                if (angleTowardsTarget < 7f) {
                    if (!isAttacking) { StartCoroutine(Attack()); }
                }
            } else {
                isAttacking = false;
                StopAllCoroutines();

                if (IsTargetInChaseRange()) {
                    MoveTowards(Target.position);
                } else {
                    MoveTowards(transform.position);
                }
            }
        }

        private IEnumerator Attack() {
            isAttacking = true;
            while (true) {
                combat.Attack();
                yield return new WaitForSeconds(1f / attacksPerSecond);
            }
        }

        private float GetDistanceToTarget() {
            if (Target == null) return 1000f;

            return Vector3.Distance(Target.position, transform.position);
        }

        private bool IsTargetInAttackRange() {
            //TODO expose combat.CurrentWeapon and get a max range from this?
            return GetDistanceToTarget() <= attackRadius;
        }
        private bool IsTargetInChaseRange() {
            return GetDistanceToTarget() <= chaseRadius;
        }

        private void LookTowardsTarget() {
            Vector3 vectorToTarget = Target.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToTarget, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(Target.position - transform.position);
        }

        public override void Die() {
            base.Die();

            StopAllCoroutines();
            Destroy(gameObject, 3f);
        }

        private void OnTargetDied() {
            Target = null;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}