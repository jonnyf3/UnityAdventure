using UnityEngine;
using RPG.Characters;
using RPG.Audio;
using RPG.States;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;

        [SerializeField] AudioClip[] damageSounds = default;
        [SerializeField] AudioClip[] deathSounds = default;
        private Voice voice;

        public delegate void OnHealthChanged(float percent);
        public event OnHealthChanged onHealthChanged;

        public delegate void OnDeath();
        public event OnDeath onDeath;

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
            voice = GetComponent<Voice>();
            CurrentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage) {
            if (IsDead) { return; }

            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);
            
            if (IsDead) {
                onDeath?.Invoke();
                voice.PlaySound(deathSounds);

                var character = GetComponent<Character>();
                character.SetState<DeadState>(new StateArgs(character));
            } else {
                voice.PlaySound(damageSounds);
            }
        }
        public void RestoreHealth(float amount) {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        }

        public void Respawn() {
            GetComponent<Animator>().SetTrigger("onRespawn");
            RestoreHealth(maxHealth);
        }
    }
}