using UnityEngine;

namespace RPG.Magic
{
    public abstract class MagicBehaviour : MonoBehaviour
    {
        public MagicData Data { protected get; set; }

        private new AudioSource audio {
            get {
                var audioSource = GetComponent<AudioSource>();
                if (audioSource == null) {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                return audioSource;
            }
        }

        public abstract void Use();


        protected void DoAnimation() {
            //TODO this behaviour isn't really an "attack", but still using the Attack animation state
            var animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Attack");
        }

        protected void DoParticleEffect() {
            var particles = Instantiate(Data.ParticleEffect, gameObject.transform);
            var duration = particles.main.duration + particles.main.startLifetime.constant;
            Destroy(particles.gameObject, duration);
        }

        protected void PlaySoundEffect() {
            //TODO add sound effects to each magic ability
            audio.PlayOneShot(Data.SoundEffect);
        }
    }
}