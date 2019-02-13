using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : AICharacter
    {
        private WeaponSystem combat;
        [Header("Combat")]
        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float turnSpeed = 2f;
        [SerializeField] float attacksPerSecond = 0.5f;

        private float distanceToTarget;
        private float attackRadius;

        enum State { idle, patrolling, chasing, attacking, dead }
        State state = State.idle;

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

        protected override void Update() {
            if (state == State.dead) { return; }

            base.Update();

            if (Target == null) { return; }
            distanceToTarget = Vector3.Distance(Target.position, transform.position);
            attackRadius = combat.CurrentWeapon.AttackRange;

            if (distanceToTarget <= attackRadius) {
                if (state != State.attacking) {
                    StopAllCoroutines();
                    StartCoroutine(Attack());
                }
            }
            else if (distanceToTarget <= chaseRadius) {
                if (state != State.chasing) {
                    StopAllCoroutines();
                    StartCoroutine(Chase());
                }
            }
            else if (distanceToTarget > chaseRadius) {
                Idle();
            }
        }

        private IEnumerator Attack() {
            state = State.attacking;

            StopMoving();
            float timeSinceLastAttack = (1f / attacksPerSecond);
            while (distanceToTarget <= attackRadius) {
                timeSinceLastAttack += Time.deltaTime;

                LookTowardsTarget();
                //Attack only when looking (roughly) towards the target
                Vector3 unitVectorToTarget = (Target.position - transform.position).normalized;
                float angleTowardsTarget = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
                
                if (timeSinceLastAttack >= (1f / attacksPerSecond) && angleTowardsTarget < 7f) {
                    combat.Attack();
                    timeSinceLastAttack = 0;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Chase() {
            state = State.chasing;
            while (distanceToTarget <= chaseRadius) {
                SetMoveTarget(Target.position);
                yield return new WaitForEndOfFrame();
            }
        }

        private void Idle() {
            if (patrolPath) {
                if (state != State.patrolling) {
                    state = State.patrolling;
                    StopAllCoroutines();
                    StartCoroutine(Patrol());
                }
            }
            else {
                if (state != State.idle) {
                    state = State.idle;
                    StopMoving();
                    StopAllCoroutines();
                }
            }
        }

        private void LookTowardsTarget() {
            Vector3 vectorToTarget = Target.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToTarget, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(Target.position - transform.position);
        }

        public override void Die() {
            base.Die();

            state = State.dead;
            StopAllCoroutines();
            Destroy(gameObject, 3f);
        }

        private void OnTargetDied() {
            Target = null;
            Idle();
        }
    }
}