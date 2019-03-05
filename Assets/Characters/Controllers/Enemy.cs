using System.Collections;
using UnityEngine;
using RPG.Combat;
using RPG.States;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class Enemy : AICharacter
    {
        private WeaponSystem combat;
        [Header("Combat")]
        [SerializeField] float chaseRadius = 5f;
        [SerializeField] float attacksPerSecond = 0.5f;

        public float ChaseRadius => chaseRadius;
        public float AttackRadius => combat.CurrentWeapon.AttackRange;
        public float AttacksPerSecond => attacksPerSecond;
        
        private Transform target;
        public Transform Target {
            get { return target; }
            set {
                if (target == value) { return; }
                //unsubscribe from previous target
                if (target) { target.GetComponent<Health>().onDeath -= OnTargetDied; }

                target = value;
                if (target) {
                    target.GetComponent<Health>().onDeath += OnTargetDied;
                } else {
                    SetState<IdleState>();
                }
            }
        }

        protected override void Start() {
            base.Start();

            combat = GetComponent<WeaponSystem>();
            Target = GetClosestTarget();
        }

        void Update() {
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

        public override void Alert(GameObject attacker) {
            Target = attacker.transform;
            if (Vector3.Distance(transform.position, Target.position) > ChaseRadius) {
                StartCoroutine(SeekAttacker());
            }
        }
        private IEnumerator SeekAttacker() {
            var startChaseRadius = ChaseRadius;
            chaseRadius = Vector3.Distance(transform.position, Target.position) + 1f;
            yield return new WaitForSeconds(5f);
            chaseRadius = startChaseRadius;
        }

        private void OnTargetDied() {
            Target = GetClosestTarget();
        }
    }
}