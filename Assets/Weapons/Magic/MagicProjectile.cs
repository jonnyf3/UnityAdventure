using UnityEngine;
using RPG.Core;

namespace RPG.Weapons
{
    public class MagicProjectile : MonoBehaviour
    {
        [SerializeField] float launchSpeed = 8f;
        public float LaunchSpeed {
            get { return launchSpeed; }
        }

        public float Damage { private get; set; }

        private void OnTriggerEnter(Collider other) {
            if (other.isTrigger) { return; }

            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }
    }
}