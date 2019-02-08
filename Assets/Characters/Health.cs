using UnityEngine;
using UnityEngine.UI;
namespace RPG.Characters
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] Slider healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        private float currentHealth;
        private AudioSource audio;

        public delegate void OnDeath();
        public event OnDeath onDeath;

        public float HealthPercent {
            get { return currentHealth / maxHealth; }
        }
        public bool IsDead {
            get { return currentHealth <= 0; }
        }

        void Start() {
            audio = GetComponent<AudioSource>();

            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        public void TakeDamage(float damage) {
            //PlaySound(damageSounds);

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            UpdateHealthBar();

            if (IsDead) { Die(); }
        }
        public void RestoreHealth(float amount) {
            TakeDamage(-amount);
        }

        private void UpdateHealthBar() {
            if (!healthBar) { return; }

            healthBar.value = HealthPercent;
        }

        private void PlaySound(AudioClip[] sounds) {
            var clip = sounds[Random.Range(0, damageSounds.Length)];
            audio.PlayOneShot(clip);
        }

        private void Die() {
            //TODO could use Character member variable rather than delegate?
            //TODO instantiate onDeath event properly so it isn't null if there are no listeners
            if (onDeath != null) onDeath();  //specific death actions for the character

            //PlaySound(deathSounds);
            var animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("onDeath");
        }
    }
}