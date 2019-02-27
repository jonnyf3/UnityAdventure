using UnityEngine;
using RPG.UI;

namespace RPG.Collectible
{
    [RequireComponent(typeof(SphereCollider))]
    public class TreasureCollector : MonoBehaviour
    {
        [SerializeField] float range = 1.5f;
        [SerializeField] float gatherSpeed = 8f;
        
        private SphereCollider collector;
        private int treasureCount = 0;
        //private static int totalTreasure = 0; //global count (to persist between scenes?)

        private HUD hud;

        void Start() {
            collector = GetComponent<SphereCollider>();
            collector.radius = range;

            hud = FindObjectOfType<HUD>();
            hud.UpdateTreasureText(treasureCount, Color.white);
            
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
            hud.UpdateTreasureText(treasureCount, color);
        }
    }
}