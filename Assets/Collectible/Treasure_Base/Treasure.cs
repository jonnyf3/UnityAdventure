using System;
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
        private Vector3 offset;
        private float speed;
        
        public static event Action<int, Color> onTreasureCollected;

        public void Attract(Transform destination, Vector3 offset, float speed) {
            this.destination = destination;
            this.offset = offset;
            this.speed = speed;
            collected = true;
        }

        private void Update() {
            if (!collected) { return; }

            //If treasure has been collected, fly towards player
            Vector3 unitVectorToPlayer = ((destination.position + offset) - transform.position).normalized;
            Vector3 gatherVelocity = unitVectorToPlayer * speed;
            GetComponent<Rigidbody>().velocity = gatherVelocity;
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.GetComponent<Player>()) {
                var color = GetComponentInChildren<MeshRenderer>().material.color;
                onTreasureCollected(value, color);

                PlayParticleEffect();
                Destroy(gameObject);
            }
        }

        private void PlayParticleEffect() {
            var particlesObj = Instantiate(collectionFX, transform.position, Quaternion.identity);
            var particles = particlesObj.GetComponent<ParticleSystem>().main;
            var duration = particles.duration + particles.startLifetime.constant;
            Destroy(particlesObj, duration);
        }
    }
}