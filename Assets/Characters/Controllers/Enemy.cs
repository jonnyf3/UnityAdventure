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
        [SerializeField] float attacksPerSecond = 0.5f;
        public float AttacksPerSecond => attacksPerSecond;
        private float attackRadius => combat.CurrentWeapon.AttackRange;

        [Header("Detection")]
        [SerializeField] float maxAwarenessDistance = 20f;
        [SerializeField] float detectionSpeed = 1f;
        private float detectionLevel = 0;

        public delegate void OnDetectionChanged(float percent);
        public event OnDetectionChanged onDetectionChanged;

        private Transform target = null;
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
            Target = GetBestTarget();
        }

        void Update() {
            if (IsDead) { return; }

            Move();

            detectionLevel = UpdateDetectionLevel();
            onDetectionChanged(detectionLevel);

            if (Target == null && !(currentState as IdleState)) {
                SetState<IdleState>();
            }

            if (detectionLevel >= 1f) {
                AlertNearby();
                if (!(currentState as AttackingState)) { SetState<ChasingState>(); }
            }
            else if (detectionLevel >= 0.5f) {
                if (!(currentState as CombatState)) { SetState<InvestigatingState>(); }
            }
            else if (detectionLevel < 0.25f) {
                if (!(currentState as IdleState)) { SetState<IdleState>(); }
            }
        }

        private float UpdateDetectionLevel() {
            float detection = detectionLevel;

            var nextTarget = GetBestTarget();
            if (nextTarget && nextTarget != Target && !(currentState as CombatState)) {
                Target = nextTarget;
                return Mathf.Max(DetectionAmount(nextTarget), 0f);
            }

            var detectionThisFrame = detectionSpeed * DetectionAmount(Target) * Time.deltaTime;
            detection += detectionThisFrame;

            return Mathf.Clamp(detection, 0, 1f);
        }

        private Transform GetBestTarget() {
            Transform nextTarget = null;
            float maxDetection = 0f;
            
            var mask = ~0;
            var objectsInRange = Physics.OverlapSphere(transform.position, maxAwarenessDistance, mask, QueryTriggerInteraction.Ignore);
            
            foreach (var obj in objectsInRange) {
                var character = obj.GetComponent<Character>();
                if (!character || IsInvalidTarget(character)) { continue; }
                
                if (DetectionAmount(character.transform) >= maxDetection) {
                    nextTarget = character.transform;
                    maxDetection = DetectionAmount(character.transform);
                }
            }

            return nextTarget;
        }
        private bool IsInvalidTarget(Character character) {
            return (character.allyState == AllyState.Neutral ||
                    character.allyState == allyState ||
                    character.IsDead);
        }

        private float DetectionAmount(Transform target) {
            //Calculate the detection amount of the target this frame
            if (!target) { return -1f; }

            var vectorToTarget = target.position - transform.position;

            //check for obstruction
            Ray ray = new Ray(transform.position + Vector3.up, vectorToTarget);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo) || hitInfo.transform != target) { return -1f; }

            //calculate perception based on whether target is in front/how far away
            var theta = Vector3.SignedAngle(vectorToTarget, transform.forward, Vector3.up);
            var perception = Mathf.Cos(theta * Mathf.Deg2Rad) / vectorToTarget.magnitude;
            
            return Mathf.Max(perception, -1f);
        }

        public override void Alert(Character attacker) {
            detectionLevel = 1f;
            if (!(currentState as CombatState)) {
                Target = attacker.transform;
                SetState<AttackingState>();
            }
        }
        private void AlertNearby() {
            var mask = ~0;
            var objectsInRange = Physics.OverlapSphere(transform.position, 5f, mask, QueryTriggerInteraction.Ignore);
            foreach (var obj in objectsInRange) {
                var character = obj.GetComponent<Character>();
                if (character && !character.IsDead && character.allyState == allyState) {
                    character.Alert(Target.GetComponent<Character>());
                }
            }
        }

        private void OnTargetDied() {
            Target = GetBestTarget();
            detectionLevel = Mathf.Max(DetectionAmount(Target), 0f);
            if (!Target) { SetState<IdleState>(); }
        }
    }
}