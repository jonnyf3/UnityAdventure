using UnityEngine;
using RPG.UI;

namespace RPG.Collectible
{
    public class TreasureCollector : MonoBehaviour
    {
        [SerializeField] float range = 1.5f;
        [SerializeField] float gatherSpeed = 8f;
        [SerializeField] Vector3 collectionOffset = Vector3.zero;
        
        private int treasureCount = 0;
        //private static int totalTreasure = 0; //global count (to persist between scenes?)

        private HUD hud;

        void Start() {
            hud = FindObjectOfType<HUD>();
            hud.UpdateTreasureText(treasureCount, Color.white);
            
            //Register for treasure collection events
            Treasure.onTreasureCollected += Collect;
        }

        private void Update() {
            var treasuresInRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Treasure"));
            foreach (var treasure in treasuresInRange) {
                if (Physics.Raycast(treasure.transform.position, transform.position + collectionOffset - treasure.transform.position,
                                    out RaycastHit hitinfo, range, ~0, QueryTriggerInteraction.Ignore)) {
                    treasure.GetComponent<Treasure>().Attract(transform, collectionOffset, gatherSpeed);
                }
            }
        }

        public void Collect(int value, Color color) {
            treasureCount += value;
            hud.UpdateTreasureText(treasureCount, color);
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
            Gizmos.DrawWireSphere(transform.position + collectionOffset, 0.15f);
        }
    }
}