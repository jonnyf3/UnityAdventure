using UnityEngine;
using UnityEngine.Assertions;
using RPG.Weapons;
using RPG.States;

namespace RPG.Characters
{
    [SelectionBase]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
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
        protected new AudioSource audio;
        protected CharacterMovement movement;
        protected Health health;

        private const string ANIMATOR_ATTACK_PARAM = "onAttack";

        protected bool focussed = false;

        public delegate void OnDeath();
        public event OnDeath onDeath;

        protected State currentState;
        public void SetState<T>(StateArgs args) where T : State {
            //already has this state behaviour
            if (GetComponent<T>() != null) { return; }

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
            audio = gameObject.AddComponent<AudioSource>();
            
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

        public void PlaySound(AudioClip clip) {
            //TODO assert clip is not empty/identify which sound clip needs setting
            if (!clip) { return; }
            audio.PlayOneShot(clip);
        }
        public void PlaySound(AudioClip[] sounds) {
            if (sounds.Length == 0)  { return; }
            var clip = sounds[Random.Range(0, sounds.Length)];
            PlaySound(clip);
        }

        public void DoCustomAnimation(AnimationClip clip) {
            Assert.IsNotNull(animOverride, gameObject + " has no animator override controller to set custom animation!");
            animOverride["DEFAULT ATTACK"] = clip;
            animator.SetTrigger(ANIMATOR_ATTACK_PARAM);
        }

        public void Focus(bool focus) { focussed = focus; }

        public virtual void Alert(GameObject attacker) {
            //Notify a character that they have been attacked (particularly for a distant ranged attack)
            //Overridden by EnemyController, not implemented for other characters
            return;
        }

        public virtual void Die() {
            SetState<DeadState>(new StateArgs(this));
            onDeath?.Invoke();
        }
    }
}