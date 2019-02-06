using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using RPG.Core;
using System;

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
            healthBar.value = HealthPercent;
        }

        public void TakeDamage(float damage) {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            healthBar.value = HealthPercent;

            if (currentHealth <= 0) { onDeath(); }
        }
        public void RestoreHealth(float amount) {
            TakeDamage(-amount);
        }
    }
}