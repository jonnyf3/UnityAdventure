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

        protected override void Update() {
            if (IsDead) { return; }

            detectionLevel = UpdateDetectionLevel();
            onDetectionChanged(detectionLevel);

            base.Update();
        }
        
        private float UpdateDetectionLevel() {
            float detection = detectionLevel;

            var nextTarget = GetBestTarget();
            if (nextTarget && nextTarget != Target && !(currentState as CombatState)) {
                Target = nextTarget;
                return Mathf.Max(DetectionAmount(nextTarget), 0f);
            }
            
            var detectionThisFrame = DetectionAmount(Target) * Time.deltaTime;
            if (detectionThisFrame > 0) { detectionThisFrame *= detectionSpeed; }
            detection += detectionThisFrame;

            return Mathf.Clamp(detection, 0, 1f);
        }

        protected override void DetermineState() {
            if (Target == null && !(currentState as IdleState)) {
                SetState<IdleState>();
            }

            if (detectionLevel >= 1f) {
                if (!(currentState as AttackingState)) {
                    AlertNearby();
                    SetState<ChasingState>();
                }
            }
            else if (detectionLevel >= 0.5f) {
                if (!(currentState as CombatState)) { SetState<InvestigatingState>(); }
            }
            else if (detectionLevel < 0.25f) {
                if (!(currentState as IdleState)) { SetState<IdleState>(); }
            }
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
            //Calculate the current perception of the target
            if (!target) { return -2f; }

            var vectorToTarget = target.position - transform.position;

            //check for obstruction (don't check too high in case of rolling!)
            Ray ray = new Ray(transform.position + (0.25f * Vector3.up), vectorToTarget);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxAwarenessDistance, ~0, QueryTriggerInteraction.Ignore) || hitInfo.transform != target) { return -1f; }

            //should have large detection if the target is very close
            if (vectorToTarget.magnitude < 1f) { return 1f; }

            //calculate perception based on whether target is in front/how far away
            var theta = Vector3.SignedAngle(vectorToTarget, transform.forward, Vector3.up);
            if (Mathf.Abs(theta) > 90f) { return -1f; }

            float angleComponent = 1.5f * Mathf.Cos(theta * Mathf.Deg2Rad) * Mathf.Cos(theta * Mathf.Deg2Rad);
            float proximityComponent = 2f /  (vectorToTarget.magnitude);

            return angleComponent * proximityComponent;
        }

        public override void Alert(Character attacker) {
            detectionLevel = 1f;
            bool newAttackerInRange = Vector3.Distance(transform.position, attacker.transform.position) <= attackRadius;
            if (!(currentState as CombatState) || newAttackerInRange) {
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