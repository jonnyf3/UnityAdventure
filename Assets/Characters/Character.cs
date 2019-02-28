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
        public void SetState<T>(StateArgs args) where T : State {
            //already has this state behaviour
            if (GetComponent<T>() != null) {
                GetComponent<T>().SetArgs(args);
                return;
            }

            //remove previous state behaviour
            if (currentState != null) {
                currentState.OnStateExit();
                Destroy(currentState);
            }

            //add new state behaviour
            currentState = gameObject.AddComponent<T>();
            currentState.OnStateEnter(args);
        }

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
        }
        
        public void GiveWeapon(WeaponData weapon) {
            if (GetComponent<WeaponSystem>()) { GetComponent<WeaponSystem>().UnlockWeapon(weapon); }
        }

        public virtual void Alert(GameObject attacker) {
            //Notify a character that they have been attacked (particularly for a distant ranged attack)
            //Overridden by EnemyController, not implemented for other characters
            return;
        }
    }
}