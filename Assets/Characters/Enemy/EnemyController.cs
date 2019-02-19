using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
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
        private Transform Target {
            get { return target; }
            set {
                if (target) { target.GetComponent<Character>().onDeath -= OnTargetDied; }
                target = value;

                if (target) {
                    target.GetComponent<Character>().onDeath += OnTargetDied;
                } else {
                    var idleArgs = new IdlingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
                    SetState<IdlingState>(idleArgs);
                }
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
            if (GetComponent<Health>().IsDead) { return; }

            base.Update();

            //TODO call less often (dont search over all FindObjectsOfType?)
            Target = GetClosestTarget();
            if (Target == null) { return; }

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
                var idleArgs = new IdlingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
                SetState<IdlingState>(idleArgs);
            }
        }
               
        public override void Alert(GameObject attacker) {
            Target = attacker.transform;
            if (Vector3.Distance(transform.position, Target.position) > chaseRadius) {
                StartCoroutine(SeekAttacker());
            }
        }
        private IEnumerator SeekAttacker() {
            var startChaseRadius = chaseRadius;
            chaseRadius = Vector3.Distance(transform.position, Target.position);
            yield return new WaitForSeconds(5f);
            chaseRadius = startChaseRadius;
        }

        public override void Die() {
            SetState<DeadState>(new StateArgs(this));
            base.Die();

            if (target) { target.GetComponent<Character>().onDeath -= OnTargetDied; }
            Destroy(gameObject, 3f);
        }

        private void OnTargetDied() {
            Target = GetClosestTarget();
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
    }
}