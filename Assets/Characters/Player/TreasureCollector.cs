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
        [SerializeField] Text displayText = null;

        private SphereCollider collector = null;
        private int treasureCount = 0;
        //private static int totalTreasure = 0; //global count (to persist between scenes?)

        // Start is called before the first frame update
        void Start() {
            collector = GetComponent<SphereCollider>();
            collector.radius = range;

            Assert.IsNotNull(displayText, "Could not find treasure text element, is it assigned?");
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

        public void Collect(int value) {
            treasureCount += value;

            UpdateText();
        }

        private void UpdateText() {
            displayText.text = treasureCount.ToString();
        }
    }
}