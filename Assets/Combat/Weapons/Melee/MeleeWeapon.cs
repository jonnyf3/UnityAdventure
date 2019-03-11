using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Combat
{
    public class MeleeWeapon : WeaponBehaviour
    {
        private Collider[] colliders;
        private void EnableColliders(bool enabled) {
            foreach (var collider in colliders) { collider.enabled = enabled; }
        }

        private void Start() {
            colliders = GetComponents<Collider>();
            Assert.IsTrue(colliders.Length > 0, "Melee weapon must have a collider!");
            EnableColliders(false);
        }

        public override void Attack() {
            //set the attacking status to true for the duration of attacking so that collisions are detected
            StartCoroutine(SetAttackingStatus());
            return;
        }

        private IEnumerator SetAttackingStatus() {
            EnableColliders(true);
            yield return new WaitForSeconds(Data.AnimClip.length);
            EnableColliders(false);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.isTrigger || other.gameObject == owner.gameObject) { return; }

            var damageable = other.GetComponent<Health>();
            if (damageable != null) {
                damageable.TakeDamage(Data.damage, owner);
                EnableColliders(false);
            }
        }
    }
}