using UnityEngine;
using UnityEngine.Assertions;
using RPG.Movement;
using RPG.States;
using RPG.Combat;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        public enum AllyState { Hostile, Ally, Neutral }
        [Header("Allegiance")]
        public AllyState allyState = default;

        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController = null;
        [SerializeField] AnimatorOverrideController animOverride = null;
        [SerializeField] Avatar avatar = null;
        
        //Standard required components
        protected Animator animator;
        protected CharacterMovement movement;
        protected Health health;
        
        protected State currentState;
        public void SetState<T>() where T : State {
            if (GetComponent<T>() != null) { return; }
            
            if (currentState != null) { Destroy(currentState); }
            currentState = gameObject.AddComponent<T>();
        }

        public bool IsDead => (currentState as DeadState);
        public bool IsFalling => (currentState as FallingState);
        public bool IsInCombat => (currentState as CombatState);

        protected virtual void Awake() {
            animator = gameObject.AddComponent<Animator>();
            
            movement = gameObject.GetComponent<CharacterMovement>();
            Assert.IsNotNull(movement, gameObject + " should have a CharacterMovement component");
        }

        protected virtual void Start() {
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = avatar;
            if (animOverride) { animator.runtimeAnimatorController = animOverride; }

            health = GetComponent<Health>();
            Assert.IsNotNull(health, gameObject + " should have a Health component");
            health.onDeath += OnDied;
            health.onTakeDamage += GetHit;

            movement.onLeftGround += () => { if (!IsDead) { SetState<FallingState>(); } };
        }

        public void GiveWeapon(Weapon weapon) {
            if (GetComponent<WeaponSystem>()) { GetComponent<WeaponSystem>().UnlockWeapon(weapon); }
        }

        private void GetHit(Character attacker) {
            Alert(attacker);
            Stagger(attacker);
        }
        public virtual void Alert(Character attacker) {
            //Notify a character that they have been attacked (particularly for a distant ranged attack)
            //Overridden by EnemyController, not implemented for other characters
            return;
        }
        private void Stagger(Character attacker) {
            var vectorFromAttacker = attacker.transform.position - transform.position;
            var attackDirection = transform.InverseTransformVector(vectorFromAttacker);

            animator.SetFloat("StaggerFront", attackDirection.z);
            animator.SetFloat("StaggerRight", attackDirection.x);
            animator.SetTrigger("onGetHit");
        }

        private void OnDied() {
            SetState<DeadState>();
        }
    }
}