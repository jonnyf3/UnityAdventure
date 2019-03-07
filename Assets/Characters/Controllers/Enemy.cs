using UnityEngine;
using RPG.Combat;
using RPG.States;
using RPG.UI;

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

        private Transform target = null;
        public Transform Target {
            get { return target; }
            set {
                if (target == value) { return; }

                if (target) { target.GetComponent<Health>().onDeath -= OnTargetDied; }
                target = value;
                if (target) {
                    target.GetComponent<Health>().onDeath += OnTargetDied;
                }
                detectionLevel = DetectionAmount(target);
            }
        }
        
        protected override void Start() {
            base.Start();

            combat = GetComponent<WeaponSystem>();
            Target = GetBestTarget();
        }

        void Update() {
            if (IsDead) { detectionLevel = 0f; return; }

            Move();
            GetComponentInChildren<CharacterUI>().UpdateDetection(detectionLevel);

            if (DetectionAmount(Target) < 0.2f && !(currentState as CombatState)) {
                Target = GetBestTarget();
                if (Target == null && !(currentState as IdleState)) {
                    SetState<IdleState>();
                }
            }
            
            detectionLevel += detectionSpeed * (DetectionAmount(Target) * Time.deltaTime);
            detectionLevel = Mathf.Clamp(detectionLevel, 0, 1f);
            
            if (detectionLevel >= 1f) {
                if (!(currentState as CombatState)) { SetState<ChasingState>(); }
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
            //Calculate the detection amount of the target this frame
            if (!target) { return -1f; }

            var vectorToTarget = target.position - transform.position;

            //check for obstruction
            Ray ray = new Ray(transform.position + Vector3.up, vectorToTarget);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo) || hitInfo.transform != target) { return -1f; }

            //calculate perception based on whether target is in front/how far away
            var theta = Vector3.SignedAngle(vectorToTarget, transform.forward, Vector3.up);
            var perception = Mathf.Cos(theta * Mathf.Deg2Rad) / vectorToTarget.magnitude;
            perception = Mathf.Max(perception, -1f);

            return perception;
        }

        public override void Alert(Character attacker) {
            Target = attacker.transform;
            if (!(currentState as AttackingState)) {
                detectionLevel = 1f;
                SetState<AttackingState>();
            }
        }

        private void OnTargetDied() {
            Target = GetBestTarget();
            if (!Target) { SetState<IdleState>(); }
        }
    }
}