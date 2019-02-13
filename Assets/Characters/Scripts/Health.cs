using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] Slider healthBar = null;
        [SerializeField] AudioClip[] damageSounds = default;
        [SerializeField] AudioClip[] deathSounds = default;

        private Character character;
        private float currentHealth;
        private new AudioSource audio;
        
        private float HealthPercent {
            get { return currentHealth / maxHealth; }
        }
        private bool IsDead {
            get { return currentHealth <= 0; }
        }

        void Start() {
            character = GetComponent<Character>();
            audio = GetComponent<AudioSource>();

            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        private void UpdateHealthBar() {
            if (!healthBar) { return; }

            healthBar.value = HealthPercent;
        }

        public void TakeDamage(float damage) {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            UpdateHealthBar();

            if (IsDead) {
                character.Die();
                PlaySound(deathSounds);
            } else {
                PlaySound(damageSounds);
            }
        }
        public void RestoreHealth(float amount) {
            TakeDamage(-amount);
        }

        private void PlaySound(AudioClip[] sounds) {
            //var clip = sounds[Random.Range(0, sounds.Length)];
            //audio.PlayOneShot(clip);
        }
    }
}