using UnityEngine;

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

            health = GetComponent<Health>();
            if (!health) { health = gameObject.AddComponent<Health>(); }

            //TODO add component and pass parameters
            movement = gameObject.GetComponent<CharacterMovement>();
            if (!movement) { movement = gameObject.AddComponent<CharacterMovement>(); }
        }

        protected virtual void Start() {
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = avatar;

            if (animOverride) { animator.runtimeAnimatorController = animOverride; }
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