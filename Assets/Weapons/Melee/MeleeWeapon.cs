using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.Weapons
{
    public class MeleeWeapon : WeaponBehaviour
    {
        private bool isAttacking = false;

        private void Start() {
            var collider = GetComponent<Collider>();
            Assert.IsNotNull(collider, "Melee weapon must have a collider!");
        }

        public override void Attack() {
            //set the attacking status to true for the duration of attacking so that collisions are detected
            StartCoroutine(SetAttackingStatus());
            return;
        }

        private IEnumerator SetAttackingStatus() {
            isAttacking = true;
            yield return new WaitForSeconds(Data.AnimClip.length);
            isAttacking = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (!isAttacking || other.isTrigger || other.gameObject == owner) { return; }

            var damageable = other.gameObject.GetComponent<Health>();
            if (damageable != null) {
                damageable.TakeDamage(Data.Damage);
                //Only damage one target per "attack"
                isAttacking = false;
            }
        }
    }
}