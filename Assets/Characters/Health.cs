using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using RPG.Core;

namespace RPG.Characters
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] Slider healthBar = null;
        private float currentHealth;

        public delegate void OnDeath();
        public event OnDeath onDeath;

        public float HealthPercent {
            get { return currentHealth / maxHealth; }
        }

        // Start is called before the first frame update
        void Start() {
            currentHealth = maxHealth;

            //TODO health bar UI may be optional?
            Assert.IsNotNull(healthBar, "Could not find a health bar UI element, is it assigned?");
        }

        public void TakeDamage(float damage) {
            currentHealth -= damage;
            Mathf.Clamp(currentHealth, 0, maxHealth);

            healthBar.value = HealthPercent;

            if (currentHealth <= 0) {
                onDeath();
            }
        }
    }
}