using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using RPG.Combat;
using RPG.Characters;

namespace RPG.UI
{
    public class CharacterUI : MonoBehaviour
    {
        private Slider healthBar;
        private Text displayText;
        [SerializeField] Image detectionMeter = null;

        private void Awake() {
            healthBar = GetComponentInChildren<Slider>();
            displayText = GetComponentInChildren<Text>();
            //detectionMeter = GetComponentInChildren<Image>();

            GetComponentInParent<Health>().onHealthChanged += UpdateHealthBar;
            GetComponentInParent<Health>().onDeath += () => gameObject.SetActive(false);
            if (GetComponentInParent<Enemy>()) {
                GetComponentInParent<Enemy>().onDetectionChanged += UpdateDetection;
            }
        }

        public void Show(bool visible) {
            float desiredAlpha = visible ? 1f : 0f;
            foreach (var uiElement in GetComponentsInChildren<Graphic>()) {
                if (uiElement == detectionMeter) { continue; }
                uiElement.CrossFadeAlpha(desiredAlpha, 0f, true);
            }
        }

        private void UpdateHealthBar(float percent) {
            //having a health bar is optional
            if (healthBar) { healthBar.value = percent; }
        }

        public void SetUIText(string text) {
            Assert.IsNotNull(displayText, "This character has no UI text which can be set");
            displayText.text = text;
        }

        private void UpdateDetection(float percent) {
            Assert.IsNotNull(detectionMeter, "No detection meter specified on this UI canvas");

            detectionMeter.fillAmount = percent;
            var alpha = (percent > 0.95f) ? 0f : 1f;
            detectionMeter.color = new Color(1f, 1f - percent, 0f, alpha);
        }

        // Align the UI so it always points at the camera
        void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }
}