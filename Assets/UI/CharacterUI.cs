using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using RPG.Combat;

namespace RPG.UI
{
    public class CharacterUI : MonoBehaviour
    {
        private Slider healthBar;
        private Text displayText;
        [SerializeField] Image detectionMeter;

        private void Awake() {
            GetComponentInParent<Health>().onHealthChanged += UpdateHealthBar;
            GetComponentInParent<Health>().onDeath += () => gameObject.SetActive(false);

            healthBar = GetComponentInChildren<Slider>();
            displayText = GetComponentInChildren<Text>();
            //detectionMeter = GetComponentInChildren<Image>();
        }

        public void Show(bool visible) {
            float desiredAlpha = visible ? 1f : 0f;
            foreach (var uiElement in GetComponentsInChildren<Graphic>()) {
                uiElement.CrossFadeAlpha(desiredAlpha, 0f, true);
            }
            //Detection meter should always be shown
            if (detectionMeter) { detectionMeter.CrossFadeAlpha(1f, 0f, false); }
        }

        private void UpdateHealthBar(float percent) {
            //having a health bar is optional
            if (healthBar) { healthBar.value = percent; }
        }

        public void SetUIText(string text) {
            Assert.IsNotNull(displayText, "");
            displayText.text = text;
        }

        public void UpdateDetection(float percent) {
            if (!detectionMeter) { return; }

            detectionMeter.fillAmount = percent;
            var alpha = (percent < 0.2f || percent > 0.95f) ? 0f : 1f;
            detectionMeter.color = new Color(1f, 1f - percent, 0f, alpha);
        }

        // Align the UI so it always points at the camera
        void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }
}