using UnityEngine;

namespace RPG.Actions
{
    //Basic scriptable object for special ability data - each specific ability will inherit from this
    public abstract class AbilityData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] Sprite sprite = null;
        [SerializeField] AnimationClip anim = null;
        [SerializeField] float cooldownTime = 5f;
        [SerializeField] ParticleSystem particleEffect = default;
        [SerializeField] AudioClip[] soundEffects = default;

        public float          CooldownTime   => cooldownTime;
        public Sprite         Sprite         => sprite;
        public ParticleSystem ParticleEffect => particleEffect;
        public AudioClip[]    SoundEffects   => soundEffects;
        public AnimationClip  AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                anim.events = new AnimationEvent[0];
                return anim;
            }
        }
        
        protected AbilityBehaviour behaviour;
        //Add the relevant AbilityBehaviour as a component
        public void AttachBehaviourTo(GameObject parent) {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(parent);
            behaviourComponent.Data = this;

            behaviour = behaviourComponent;
        }
        //this abstract method is implemented to get the specific behaviour type
        protected abstract AbilityBehaviour GetBehaviourComponent(GameObject parent);

        //Allow player to access behaviour via the scriptable object
        public void Use() { behaviour.Use(); }
    }
}