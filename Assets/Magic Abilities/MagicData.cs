using UnityEngine;

namespace RPG.Magic
{
    //Basic scriptable object for magic ability data - each specific ability will inherit from this
    public abstract class MagicData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] Sprite sprite = null;
        [SerializeField] AnimationClip anim = null;
        [SerializeField] float cooldownTime = 5f;
        [SerializeField] ParticleSystem particleEffect = default;
        [SerializeField] AudioClip[] soundEffects = default;

        public float          CooldownTime   { get { return cooldownTime; } }
        public Sprite         Sprite         { get { return sprite; } }
        public ParticleSystem ParticleEffect { get { return particleEffect; } }
        public AudioClip[]    SoundEffects   { get { return soundEffects; } }
        public AnimationClip  AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                anim.events = new AnimationEvent[0];
                return anim;
            }
        }
        

        protected MagicBehaviour behaviour;
        //Add the relevant MagicBehaviour as a component
        public void AttachBehaviourTo(GameObject parent) {
            MagicBehaviour behaviourComponent = GetBehaviourComponent(parent);
            behaviourComponent.Data = this;

            behaviour = behaviourComponent;
        }
        //this abstract method is implemented to get the specific behaviour type
        protected abstract MagicBehaviour GetBehaviourComponent(GameObject parent);

        //Allow player to access behaviour via the scriptable object
        public void Use() { behaviour.Use(); }
    }
}