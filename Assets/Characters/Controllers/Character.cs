using System;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Movement;
using RPG.States;
using RPG.Combat;
using RPG.Audio;

namespace RPG.Characters
{
    [SelectionBase]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Voice))]
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

        public virtual event Action onEnterIdleState;

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
        }

        public void SetState<T>() where T : State {
            //already has this state behaviour
            if (GetComponent<T>() != null) { return; }

            //remove previous state behaviour
            if (currentState != null) {
                Destroy(currentState);
            }

            //stop any previous motion passed to the animator
            //TODO this can be quite abrupt
            animator.SetFloat("Forward", 0);
            animator.SetFloat("Horizontal", 0);

            //add new state behaviour
            currentState = gameObject.AddComponent<T>();
            currentState.OnStateEnter();
        }

        public void GiveWeapon(WeaponData weapon) {
            if (GetComponent<WeaponSystem>()) { GetComponent<WeaponSystem>().UnlockWeapon(weapon); }
        }

        public virtual void Alert(GameObject attacker) {
            //Notify a character that they have been attacked (particularly for a distant ranged attack)
            //Overridden by EnemyController, not implemented for other characters
            return;
        }

        private void OnDied() {
            SetState<DeadState>();
        }
    }
}