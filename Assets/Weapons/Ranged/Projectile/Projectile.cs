using UnityEngine;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public GameObject EndEffect { protected get; set; }
        private float maxLifetime = 2.5f;

        private void Start() {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
            Destroy(gameObject, maxLifetime);
        }

        protected virtual void OnTriggerEnter(Collider other) {
            if (other.isTrigger) { return; }

            //Projectiles should not ricochet
            if (EndEffect) { PlayParticleEffect(); }
            Destroy(gameObject);
        }

        private void PlayParticleEffect() {
            var impactEffect = Instantiate(EndEffect, transform.position, Quaternion.identity);
            var particles = impactEffect.GetComponent<ParticleSystem>().main;
            var duration = particles.duration + particles.startLifetime.constant;
            Destroy(impactEffect, duration);
        }
    }
}