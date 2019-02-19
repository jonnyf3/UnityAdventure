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

        enum State { passive, chasing, attacking, dead }
        State state = State.passive;

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
            
            var player = FindObjectOfType<PlayerController>();
            Assert.IsNotNull(player, "Could not find player in the scene!");

            Target = GetClosestTarget();
        }

        protected override void Update() {
            if (state == State.dead) { return; }
            base.Update();

            //TODO call less often (dont search over all FindObjectsOfType?)
            Target = GetClosestTarget();
            if (Target == null) { Idle(); return; }

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
                if (state != State.passive) { Idle(); }
            }
        }

        private IEnumerator Attack() {
            state = State.attacking;

            StopMoving();
            movement.AnimatorForwardCap = 1f;
            float timeSinceLastAttack = (1f / attacksPerSecond);
            while (distanceToTarget <= attackRadius) {
                timeSinceLastAttack += Time.deltaTime;

                LookTowardsTarget();
                //Attack only when looking (roughly) towards the target
                Vector3 unitVectorToTarget = (Target.position - transform.position).normalized;
                float angleTowardsTarget = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
                if (angleTowardsTarget < 7f) {
                    //Check if shot to target is clear
                    yield return StartCoroutine(MoveToClearLineOfSight());
                    
                    //Randomise attack frequency
                    var attackVariance = Random.Range(0.9f, 1.1f);
                    var attackPeriod = (1f / attacksPerSecond) * attackVariance;
                    if (timeSinceLastAttack >= attackPeriod) {
                        combat.Attack();
                        timeSinceLastAttack = 0;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator MoveToClearLineOfSight() {
            focussed = true;
            var direction = Mathf.Sign(Random.Range(-1f, 1f));

            while (IsShotBlocked()) {
                MoveAroundTarget(direction);
                yield return new WaitForEndOfFrame();
            }

            focussed = false;
            StopMoving();
        }

        private bool IsShotBlocked() {
            int mask = ~0;
            Vector3 unitVectorToTarget = (Target.position - transform.position).normalized;
            var hit = Physics.Raycast(transform.position + new Vector3(0, 1f, 0),
                                      unitVectorToTarget, out RaycastHit hitInfo,
                                      attackRadius, mask, QueryTriggerInteraction.Ignore);
            if (!hit) { return false; }
            return hitInfo.collider.transform != Target;
        }
        private void MoveAroundTarget(float direction) {
            /* Constantly set a move target directly perpendicular to the character - combined with
            a rotation to look towards the target, this results in circular movement around the target */
            Vector3 unitVectorToTarget = (Target.position - transform.position).normalized;
            transform.forward = Vector3.ProjectOnPlane(unitVectorToTarget, Vector3.up);
            
            //new position needs to be further away than stopping distance
            var newPos = 4 * (direction * transform.right);
            SetMoveTarget(transform.position + newPos);
        }

        private IEnumerator Chase() {
            state = State.chasing;
            focussed = false;
            movement.AnimatorForwardCap = 1f;

            while (distanceToTarget <= chaseRadius) {
                SetMoveTarget(Target.position);
                yield return new WaitForEndOfFrame();
            }
        }

        private void Idle() {
            state = State.passive;
            movement.AnimatorForwardCap = 0.5f;
            StopAllCoroutines();
            if (patrolPath) {
                StartCoroutine(Patrol());
            }
        }

        private Transform GetClosestTarget() {
            Transform closestTarget = null;
            float closestDistance = 1000f;

            var characters = FindObjectsOfType<Character>();
            foreach (var character in characters) {
                if (character.allyState == AllyState.Neutral ||
                    character.allyState == allyState) { continue; }

                if (character.GetComponent<Health>().IsDead) { continue; }

                if (Vector3.Distance(transform.position, character.transform.position) <= closestDistance) {
                    closestTarget = character.transform;
                    closestDistance = Vector3.Distance(transform.position, character.transform.position);
                }
            }

            return closestTarget;
        }

        private void LookTowardsTarget() {
            Vector3 vectorToTarget = Target.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToTarget, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(Target.position - transform.position);
        }

        public override void Alert(GameObject attacker) {
            print("Alerted!");
            Target = attacker.transform;
            if (Vector3.Distance(transform.position, Target.position) > chaseRadius) {
                StartCoroutine(ChaseAttacker());
            }
        }
        private IEnumerator ChaseAttacker() {
            print("Force chasing!");
            var startChaseRadius = chaseRadius;
            chaseRadius = Vector3.Distance(transform.position, Target.position);
            yield return new WaitForSeconds(1f);
            //TODO this coroutine is stopped by StopAllCoroutines when state changes to Chasing, so never returns to shrink the chaseRadius
            chaseRadius = startChaseRadius;
        }

        public override void Die() {
            state = State.dead;
            StopAllCoroutines();
            base.Die();

            if (target) { target.GetComponent<Character>().onDeath -= OnTargetDied; }
            Destroy(gameObject, 3f);
        }

        private void OnTargetDied() {
            Target = GetClosestTarget();
            Idle();
        }
    }
}