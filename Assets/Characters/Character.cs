using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [SelectionBase]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animOverride = null;

        //Standard required components
        protected Animator animator;
        protected AudioSource audio;
        protected Health health;

        //Components which need to be overridden
        protected CharacterMovement movement;

        private const string ANIMATOR_PARAM = "Attack";

        // Start is called before the first frame update
        protected virtual void Awake() {
            health = GetComponent<Health>();
            if (!health) { health = gameObject.AddComponent<Health>(); }

            audio = GetComponent<AudioSource>();
            if (!audio) { audio = gameObject.AddComponent<AudioSource>(); }

            animator = GetComponentInChildren<Animator>();
            Assert.IsNotNull(animator, "Characters should have an animator on their body");
        }

        protected virtual void Start() {
            if (animOverride) { animator.runtimeAnimatorController = animOverride; }
        }
        
        public void DoCustomAnimation(AnimationClip clip) {
            animOverride["DEFAULT ATTACK"] = clip;
            animator.SetTrigger(ANIMATOR_PARAM);
        }
    }
}