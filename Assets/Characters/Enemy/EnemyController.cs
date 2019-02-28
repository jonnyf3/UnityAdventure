using System.Collections;
using UnityEngine;
using RPG.Combat;
using RPG.States;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : AICharacter
    {
        private WeaponSystem combat;
        [Header("Combat")]
        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attacksPerSecond = 0.5f;

        private float distanceToTarget;
        private float attackRadius;
        
        private Transform target;
        public Transform Target {
            get { return target; }
            set {
                if (target == value) { return; }

                if (target) { target.GetComponent<Health>().onDeath -= OnTargetDied; }
                target = value;
                if (target) { target.GetComponent<Health>().onDeath += OnTargetDied; }
            }
        }

        protected override void Start() {
            base.Start();

            combat = GetComponent<WeaponSystem>();
            Target = GetClosestTarget();
        }

        void Update() {
            if (GetComponent<Health>().IsDead) { return; }

            Move();

            //TODO call less often (dont search over all FindObjectsOfType?)
            Target = GetClosestTarget();
            if (Target == null) {
                DoPassiveBehaviour();
                return;
            }

            distanceToTarget = Vector3.Distance(Target.position, transform.position);
            attackRadius = combat.CurrentWeapon.AttackRange;

            if (distanceToTarget <= attackRadius) {
                var attackArgs = new AttackingStateArgs(this, Target, combat, attacksPerSecond);
                SetState<AttackingState>(attackArgs);
            }
            else if (distanceToTarget <= chaseRadius) {
                var chaseArgs = new ChasingStateArgs(this, Target);
                SetState<ChasingState>(chaseArgs);
            }
            else if (distanceToTarget > chaseRadius) {
                DoPassiveBehaviour();
            }
        }

        private void DoPassiveBehaviour() {
            if (patrolPath) {
                var patrolArgs = new PatrollingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
                SetState<PatrollingState>(patrolArgs);
            } else {
                SetState<IdleState>(new StateArgs(this));
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

        public override void Alert(GameObject attacker) {
            Target = attacker.transform;
            if (Vector3.Distance(transform.position, Target.position) > chaseRadius) {
                StartCoroutine(SeekAttacker());
            }
        }
        private IEnumerator SeekAttacker() {
            var startChaseRadius = chaseRadius;
            chaseRadius = Vector3.Distance(transform.position, Target.position) + 1f;
            yield return new WaitForSeconds(5f);
            chaseRadius = startChaseRadius;
        }

        private void OnTargetDied() {
            Target = GetClosestTarget();
        }
    }
}