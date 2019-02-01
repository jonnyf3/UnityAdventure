using UnityEngine;
using RPG.Core;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float damage = 10f;

        [SerializeField] float speed = 4f;
        public float Speed {
            get { return speed; }
        }

        private GameObject shooter;
        public GameObject Shooter {
            set { shooter = value; }
        }

        private void OnCollisionEnter(Collision collision) {
            //Prevent dealing damage to projectile's original shooter
            if (collision.gameObject != shooter) {
                var damageable = collision.transform.GetComponentInParent<IDamageable>();
                if (damageable != null) {
                    damageable.TakeDamage(damage);
                }
            }

            //Projectiles should not ricochet
            Destroy(gameObject);
        }
    }
}