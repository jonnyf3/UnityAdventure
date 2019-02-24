using System.Collections;
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

        //need to access both the fill and the background parts of the slider
        private Image[] imageComponents;
        
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
            if (healthBar) { imageComponents = healthBar.GetComponentsInChildren<Image>(); }

            CurrentHealth = maxHealth;
        }

        private void UpdateHealthBar() {
            if (!healthBar) { return; }

            StopAllCoroutines();
            EnableHealthBar();

            healthBar.value = HealthPercent;
            StartCoroutine(FadeHealthBar());
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

        private void EnableHealthBar() {
            //Set health bar opacity to full
            foreach (var image in imageComponents) {
                image.color = MakeOpaque(image.color);
            }
        }
        //TODO duplicated in TreasureCollector.cs - could move to a UI script?
        private Color MakeOpaque(Color color) {
            color.a = 1f;
            return color;
        }

        private IEnumerator FadeHealthBar() {
            yield return new WaitForSeconds(3f);
            while (healthBar.image.color.a >= 0f) {
                foreach (var image in imageComponents) {
                    image.color = Fade(image.color);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        private Color Fade(Color color) {
            color.a -= 0.5f * Time.deltaTime;
            return color;
        }
    }
}