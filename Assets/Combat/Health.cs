using System;
using UnityEngine;
using RPG.Characters;
using RPG.Saving;

namespace RPG.Combat
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float maxHealth = 100f;
        private float currentHealth;

        private float HealthPercent => currentHealth / maxHealth;
        public bool IsDead => (currentHealth <= 0);

        public event Action<float> onHealthChanged;
        public event Action<Character> onTakeDamage;
        public event Action onDeath;

        void Start() {
            currentHealth = maxHealth;
            onHealthChanged(HealthPercent);
        }
        
        public void TakeDamage(float damage, Character attacker) {
            if (IsDead) { return; }

            if (attacker) {
                //Don't allow friendly fire
                if (attacker.allyState == GetComponent<Character>().allyState) { return; }

                if (IsStealthAttack()) {
                    print("Stealth attack!");
                    damage *= 1.5f; //TODO attacker.StealthMultiplier?
                }
            }

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            onHealthChanged(HealthPercent);

            if (IsDead) {
                //TODO if attacker == Player, give EXP
                onDeath();
            } else if (attacker) {
                onTakeDamage(attacker);
            }
        }
        public void RestoreHealth(float amount) {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            onHealthChanged(HealthPercent);
        }

        private bool IsStealthAttack() {
            return GetComponent<Enemy>() ? !GetComponent<Enemy>().IsInCombat : false;
        }

        public void Respawn() {
            GetComponent<Animator>().SetTrigger("onRespawn");
            RestoreHealth(maxHealth);
        }

        #region SaveLoad
        public object SaveState() {
            return currentHealth;
        }
        public void LoadState(object state) {
            currentHealth = (float)state;
        }
        #endregion
    }
}