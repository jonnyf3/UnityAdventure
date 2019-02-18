using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Health : MonoBehaviour
    {
        private Character character;

        [SerializeField] float maxHealth = 100f;
        [SerializeField] Slider healthBar = null;
        [SerializeField] AudioClip[] damageSounds = default;
        [SerializeField] AudioClip[] deathSounds = default;
        
        private float currentHealth;
        private float CurrentHealth {
            get { return currentHealth; }
            set {
                currentHealth = value;
                UpdateHealthBar();
            }
        }
        private float HealthPercent {
            get { return CurrentHealth / maxHealth; }
        }
        public bool IsDead {
            get { return CurrentHealth <= 0; }
        }

        void Start() {
            character = GetComponent<Character>();

            CurrentHealth = maxHealth;
        }

        private void UpdateHealthBar() {
            if (!healthBar) { return; }

            healthBar.value = HealthPercent;
        }

        public void TakeDamage(float damage) {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);

            if (IsDead) {
                character.Die();
                character.PlaySound(deathSounds);
            } else {
                character.PlaySound(damageSounds);
            }
        }
        public void RestoreHealth(float amount) {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        }
    }
}