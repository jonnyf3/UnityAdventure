using UnityEngine;
using RPG.Core;

namespace RPG.Weapons
{
    public class MagicProjectile : MonoBehaviour
    {
        public float Damage { private get; set; }
        public GameObject EndEffect { private get; set; }

        private void OnTriggerEnter(Collider other) {
            if (other.isTrigger) { return; }

            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(Damage);
            }
            Instantiate(EndEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}