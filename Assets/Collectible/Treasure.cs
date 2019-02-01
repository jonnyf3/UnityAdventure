using UnityEngine;
using RPG.Characters;

namespace RPG.Collectible
{
    public class Treasure : MonoBehaviour
    {
        [SerializeField] int value = 5;
        [SerializeField] GameObject collectionFX = null;

        private bool collected = false;
        private Transform destination;
        private float speed;

        public delegate void OnTreasureCollected(int value);
        public static event OnTreasureCollected onTreasureCollected;

        public void Attract(Transform destination, float speed) {
            this.destination = destination;
            this.speed = speed;
            collected = true;
        }

        private void Update() {
            if (!collected) { return; }

            //If treasure has been collected, fly towards player
            Vector3 unitVectorToPlayer = (destination.position - transform.position).normalized;
            Vector3 gatherVelocity = unitVectorToPlayer * speed;
            GetComponent<Rigidbody>().velocity = gatherVelocity;
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.GetComponent<Player>()) {
                onTreasureCollected(value);

                Instantiate(collectionFX, transform.position, Quaternion.identity);
                //TODO remove particles gameobject from scene after firing
                Destroy(gameObject);
            }
        }
    }
}