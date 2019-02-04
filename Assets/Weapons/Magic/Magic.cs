using UnityEngine;

namespace RPG.Weapons
{
    //Basic scriptable object for magic ability data - each specific ability will inherit from this
    public abstract class MagicData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] float energyCost = 5f;
        [SerializeField] AnimationClip anim = null;

        protected IMagicBehaviour behaviour;

        public float EnergyCost { get { return energyCost; } }
        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                anim.events = new AnimationEvent[0];
                return anim;
            }
        }

        //Magic should add a behaviour component to whichever gameobject is implementing them
        abstract public void AttachComponentTo(GameObject parent);

        //Allow player to access behaviour via the scriptable object
        public void Use() { behaviour.Use(); }
    }

    //Basic interface for magic ability behaviour
    public interface IMagicBehaviour
    {
        void Use();
    }
}