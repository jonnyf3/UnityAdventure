using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Magic
{
    public abstract class MagicBehaviour : MonoBehaviour
    {
        public MagicData Data { protected get; set; }

        public abstract void Use();


        protected void DoParticleEffect() {
            var particles = Instantiate(Data.ParticleEffect, gameObject.transform);
            var duration = particles.main.duration + particles.main.startLifetime.constant;
            Destroy(particles.gameObject, duration);
        }

        protected void PlaySoundEffect() {
            //TODO add sound effects to each magic ability
            var audioSource = GetComponent<AudioSource>();
            Assert.IsNotNull(audioSource);

            audioSource.PlayOneShot(Data.SoundEffect);
        }
    }
}