using UnityEngine;
using RPG.Combat;

namespace RPG.Audio
{
    public class Voice : MonoBehaviour
    {
        private new AudioSource audio;

        [SerializeField] AudioClip[] damageSounds = default;
        [SerializeField] AudioClip[] deathSounds = default;

        private void Awake() {
            audio = gameObject.AddComponent<AudioSource>();

            var health = GetComponent<Health>();
            health.onTakeDamage += (attacker) => PlaySound(damageSounds);
            health.onDeath += () => PlaySound(deathSounds);
        }

        public void PlaySound(AudioClip clip) {
            //TODO assert clip is not empty/identify which sound clip needs setting
            if (!clip) { return; }
            audio.PlayOneShot(clip);
        }
        public void PlaySound(AudioClip[] sounds) {
            if (sounds.Length == 0) { return; }
            var clip = sounds[Random.Range(0, sounds.Length)];
            PlaySound(clip);
        }
    }
}