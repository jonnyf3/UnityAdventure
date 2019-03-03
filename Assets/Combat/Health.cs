using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;
        private float currentHealth;

        private float HealthPercent => currentHealth / maxHealth;
        public bool IsDead => (currentHealth <= 0);

        public delegate void OnHealthChanged(float percent);
        public event OnHealthChanged onHealthChanged;

        public delegate void OnTakeDamage();
        public event OnTakeDamage onTakeDamage;

        public delegate void OnDeath();
        public event OnDeath onDeath;

        void Start() {
            currentHealth = maxHealth;
            onHealthChanged(HealthPercent);
        }
        
        public void TakeDamage(float damage) {
            if (IsDead) { return; }

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            onHealthChanged(HealthPercent);

            if (IsDead) {
                onDeath();
            } else {
                onTakeDamage();
            }
        }
        public void RestoreHealth(float amount) {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            onHealthChanged(HealthPercent);
        }

        public void Respawn() {
            GetComponent<Animator>().SetTrigger("onRespawn");
            RestoreHealth(maxHealth);
        }
    }
}