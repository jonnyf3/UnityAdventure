using UnityEngine;
using RPG.Characters;

namespace RPG.Weapons
{
    public class DamageProjectile : Projectile
    {
        public float      Damage { private get; set; }
        public GameObject Owner  { private get; set; }

        protected override void OnTriggerEnter(Collider other) {
            if (other.isTrigger) { return; }

            //Prevent dealing damage to projectile's original shooter
            if (other.gameObject != Owner) {
                var damageable = other.gameObject.GetComponent<Health>();
                if (damageable != null) {
                    damageable.TakeDamage(Damage);
                }
            }

            base.OnTriggerEnter(other);
        }
    }
}