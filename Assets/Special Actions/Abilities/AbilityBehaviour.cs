using UnityEngine;
using RPG.Audio;

namespace RPG.Actions
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        public AbilityData Data { protected get; set; }

        public abstract void Use();
        
        protected void PlaySoundEffect() {
            //TODO add sound effects to each ability
            GetComponent<Voice>().PlaySound(Data.SoundEffects);
        }

        protected void AbilityUsed() {
            GetComponent<SpecialAbilities>().AbilityUsed();
        }
    }
}