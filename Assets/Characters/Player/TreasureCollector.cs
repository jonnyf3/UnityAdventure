using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using RPG.Collectible;

namespace RPG.Characters
{
    [RequireComponent(typeof(SphereCollider))]
    public class TreasureCollector : MonoBehaviour
    {
        [SerializeField] float range = 1.5f;
        [SerializeField] float gatherSpeed = 8f;

        [SerializeField] GameObject ui = null;
        private Image uiImage;
        private Text uiText;

        private SphereCollider collector;
        private int treasureCount = 0;
        //private static int totalTreasure = 0; //global count (to persist between scenes?)

        void Start() {
            collector = GetComponent<SphereCollider>();
            collector.radius = range;

            uiImage = ui.GetComponentInChildren<Image>();
            uiText  = ui.GetComponentInChildren<Text>();
            UpdateText();

            //Register for treasure collection events
            Treasure.onTreasureCollected += Collect;
        }

        private void OnTriggerEnter(Collider other) {
            var treasure = other.GetComponent<Treasure>();
            if (treasure) {
                treasure.Attract(transform, gatherSpeed);
            }
        }

        public void Collect(int value, Color color) {
            treasureCount += value;
            UpdateText();
            uiImage.color = color;
        }

        private void UpdateText() {
            Assert.IsNotNull(ui, "Could not find treasure text element, is it assigned?");

            StopAllCoroutines();
            EnableUI();

            uiText.text = treasureCount.ToString();
            StartCoroutine(FadeUI());
        }

        private void EnableUI() {
            //Set opacity to full
            uiImage.color = MakeOpaque(uiImage.color);
            uiText.color  = MakeOpaque(uiText.color);
        }
        private Color MakeOpaque(Color color) {
            color.a = 1f;
            return color;
        }

        private IEnumerator FadeUI() {
            yield return new WaitForSeconds(3f);
            while (uiImage.color.a >= 0f) {
                uiImage.color = Fade(uiImage.color);
                uiText.color  = Fade(uiText.color);

                yield return new WaitForEndOfFrame();
            }
        }
        private Color Fade(Color color) {
            color.a -= 0.5f * Time.deltaTime;
            return color;
        }
    }
}