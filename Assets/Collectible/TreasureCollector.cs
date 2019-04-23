using UnityEngine;
using RPG.UI;
using RPG.Saving;

namespace RPG.Collectible
{
    public class TreasureCollector : MonoBehaviour, ISaveable
    {
        [SerializeField] float range = 1.5f;
        [SerializeField] float gatherSpeed = 8f;
        [SerializeField] Vector3 collectionOffset = Vector3.zero;
        
        //private static int totalTreasure = 0; //global count (to persist between scenes?)
        private int treasureCount = 0;
        private Color lastColor = Color.white;
        
        void Start() {
            FindObjectOfType<HUD>().UpdateTreasureText(treasureCount, Color.white);
            
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
            lastColor = color;
            FindObjectOfType<HUD>().UpdateTreasureText(treasureCount, color);
        }
        
        public void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
            Gizmos.DrawWireSphere(transform.position + collectionOffset, 0.15f);
        }

        #region SaveLoad
        public object SaveState() {
            return new SaveStateData(treasureCount, lastColor);
        }
        public void LoadState(object state) {
            var s = (SaveStateData)state;
            var c = s.color.ToVector();
            Collect(s.count, new Color(c.x, c.y, c.z));
        }

        [System.Serializable]
        private struct SaveStateData
        {
            public int count;
            public SerializableVector3 color;

            public SaveStateData(int count, Color color) {
                this.count = count;
                this.color = new SerializableVector3(color.r, color.g, color.b);
            }
        }
        #endregion
    }
}