using RPG.CameraUI;
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
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animOverride;
        [SerializeField] Avatar avatar;

        //Standard required components
        protected Animator animator;
        protected AudioSource audio;
        protected CharacterMovement movement;
        protected Health health;

        protected CharacterUI ui;

        private const string ANIMATOR_PARAM = "Attack";

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

            //TODO these features are for enemies or NPCs but not player?
            ui = GetComponentInChildren<CharacterUI>();
            DeactivateUI();
            var viewer = Camera.main.GetComponent<Viewer>();
            viewer.onChangedFocus += DeactivateUI;

            health.onDeath += Die;
        }
        
        public void DoCustomAnimation(AnimationClip clip) {
            animOverride["DEFAULT ATTACK"] = clip;
            animator.SetTrigger(ANIMATOR_PARAM);
        }

        protected virtual void Die() { }

        public virtual void ActivateUI() {
            if (!ui) { return; }
            ui.gameObject.SetActive(true);
        }
        protected virtual void DeactivateUI() {
            if (!ui) { return; }
            ui.gameObject.SetActive(false);
        }
    }
}