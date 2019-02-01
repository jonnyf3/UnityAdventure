using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Magic")]
    public class Magic : ScriptableObject
    {
        [SerializeField] GameObject magicFX = null;
        [SerializeField] AnimationClip anim = null;
        [SerializeField] float energyCost = 5f;
        [SerializeField] float damage = 20f;

        public GameObject Effect {
            get { return magicFX; }
        }
        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                anim.events = new AnimationEvent[0];

                return anim;
            }
        }

        public float EnergyCost {
            get { return energyCost; }
        }
        public float Damage {
            get { return damage; }
        }
    }
}