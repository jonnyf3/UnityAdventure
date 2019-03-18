using UnityEngine;
using RPG.Characters;

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

        public delegate void OnTakeDamage(Character attacker);
        public event OnTakeDamage onTakeDamage;

        public delegate void OnDeath();
        public event OnDeath onDeath;

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
    }
}