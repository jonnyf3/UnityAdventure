using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [SelectionBase]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController = null;
        [SerializeField] AnimatorOverrideController animOverride = null;
        [SerializeField] Avatar avatar = null;

        //Standard required components
        protected Animator animator;
        protected new AudioSource audio;
        protected CharacterMovement movement;
        protected Health health;

        private const string ANIMATOR_PARAM = "Attack";

        public delegate void OnDeath();
        public event OnDeath onDeath;

        // Start is called before the first frame update
        protected virtual void Awake() {
            animator = gameObject.AddComponent<Animator>();
            audio = gameObject.AddComponent<AudioSource>();

            //TODO add component at runtime?
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
        
        public void DoCustomAnimation(AnimationClip clip) {
            animOverride["DEFAULT ATTACK"] = clip;
            animator.SetTrigger(ANIMATOR_PARAM);
        }

        public virtual void Die() {
            onDeath?.Invoke();
            animator.SetTrigger("onDeath");
        }
    }
}