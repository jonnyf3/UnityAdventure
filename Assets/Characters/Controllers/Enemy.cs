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
        [SerializeField] float detectionRange = 5f;
        [SerializeField] float attacksPerSecond = 0.5f;
        public float AttacksPerSecond => attacksPerSecond;
        private float attackRadius => combat.CurrentWeapon.AttackRange;
        
        [SerializeField] float detectionLevel = 0;

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
            if (currentState as DeadState) { return; }

            Move();

            if (detectionLevel < 0.1f) {
                Target = GetClosestTarget();
                if (Target == null) {
                    if (!(currentState as IdleState)) { SetState<IdleState>(); }
                    return;
                }
                detectionLevel = DetectionAmount(Target);
            }

            //TODO dection speed?
            detectionLevel += (DetectionAmount(Target) * Time.deltaTime);
            detectionLevel = Mathf.Clamp(detectionLevel, 0, 1f);

            if (detectionLevel >= 1f) {
                if (!(currentState as CombatState)) { SetState<ChasingState>(); }
            }
            else if (detectionLevel < 0.25f) {
                if (!(currentState as IdleState)) { SetState<IdleState>(); }
            }
        }

        private Transform GetClosestTarget() {
            //TODO base on detection amount rather than distance? (spherecast to nearby?)
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

        private float DetectionAmount(Transform target) {
            //Calculate the detection amount of the target this frame
            var vectorToTarget = target.position - transform.position;

            //limit max detection range
            //if (vectorToTarget.magnitude >= detectionRange && (currentState as CombatState)) { return -1f; }

            //check for obstruction
            Ray ray = new Ray(transform.position + Vector3.up, vectorToTarget);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo) || hitInfo.transform != target) { return -1f; }

            //check whether target is behind
            var theta = Vector3.SignedAngle(vectorToTarget, transform.forward, Vector3.up);
            var perception = Mathf.Cos(theta * Mathf.Deg2Rad) / vectorToTarget.magnitude;
            perception = Mathf.Max(perception, -1f);

            return perception;
        }

        public override void Alert(Character attacker) {
            //TODO only if not already in a CombatState?
            Target = attacker.transform;
            detectionLevel = 1f;
            SetState<AttackingState>();
        }

        private void OnTargetDied() {
            Target = GetClosestTarget();
            detectionLevel = DetectionAmount(Target);
        }
    }
}