using UnityEngine;
using RPG.Characters;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public float Damage { private get; set; }
        public GameObject EndEffect { private get; set; }

        public GameObject Owner { private get; set; }

        private void Start() {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.isTrigger) { return; }

            //Prevent dealing damage to projectile's original shooter
            if (other.gameObject != Owner) {
                var damageable = other.gameObject.GetComponent<Health>();
                if (damageable != null) {
                    damageable.TakeDamage(Damage);
                }
            }

            //Projectiles should not ricochet
            if (EndEffect) { PlayParticleEffect(); }
            Destroy(gameObject);
        }

        private void PlayParticleEffect() {
            var explosion = Instantiate(EndEffect, transform.position, Quaternion.identity);
            var particles = explosion.GetComponent<ParticleSystem>().main;
            var duration = particles.duration + particles.startLifetime.constant;
            Destroy(explosion, duration);
        }
    }
}