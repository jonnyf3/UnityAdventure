using UnityEngine;
using RPG.Characters;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        private Character character;

        [SerializeField] float maxHealth = 100f;
        [SerializeField] AudioClip[] damageSounds = default;
        [SerializeField] AudioClip[] deathSounds = default;

        public delegate void OnHealthChanged(float percent);
        public event OnHealthChanged onHealthChanged;
                
        private float currentHealth;
        private float CurrentHealth {
            get { return currentHealth; }
            set {
                currentHealth = value;
                onHealthChanged(HealthPercent);
            }
        }

        private float HealthPercent => CurrentHealth / maxHealth;
        public bool IsDead => (CurrentHealth <= 0);

        void Start() {
            character = GetComponent<Character>();
            CurrentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage) {
            if (IsDead) { return; }

            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);
            
            if (IsDead) {
                //TODO set death state from within here?
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