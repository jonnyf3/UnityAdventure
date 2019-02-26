using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class CharacterUI : MonoBehaviour
    {
        private Slider healthBar;
        private Text displayText;

        private void Awake() {
            GetComponentInParent<Health>().onHealthChanged += UpdateHealthBar;
            
            healthBar = GetComponentInChildren<Slider>();
            displayText = GetComponentInChildren<Text>();
        }

        public void Show(bool visible) {
            float desiredAlpha = visible ? 1f : 0f;
            foreach (var uiElement in GetComponentsInChildren<Graphic>()) {
                uiElement.CrossFadeAlpha(desiredAlpha, 0f, true);
            }
        }

        private void UpdateHealthBar(float percent) {
            //having a health bar is optional
            if (healthBar) { healthBar.value = percent; }
        }

        public void SetUIText(string text) {
            Assert.IsNotNull(displayText, "");
            displayText.text = text;
        }

        // Align the UI so it always points at the camera
        void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }
}