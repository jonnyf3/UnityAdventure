using UnityEngine;
using RPG.Characters;

namespace RPG.Actions
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        public AbilityData Data { protected get; set; }

        public abstract void Use();
        
        protected void DoParticleEffect() {
            var particles = Instantiate(Data.ParticleEffect, gameObject.transform);
            var duration = particles.main.duration + particles.main.startLifetime.constant;
            Destroy(particles.gameObject, duration);
        }

        protected void PlaySoundEffect() {
            //TODO add sound effects to each ability
            GetComponent<Character>().PlaySound(Data.SoundEffects);
        }

        protected void AbilityUsed() {
            GetComponent<SpecialAbilities>().AbilityUsed();
        }
    }
}