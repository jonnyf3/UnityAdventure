using UnityEngine;
using System.Collections.Generic;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animOverride = null;
        private Animator animator = null;

        [SerializeField] Weapon currentWeapon = null;
        [SerializeField] Transform weaponHand = null;
        
        // Start is called before the first frame update
        void Start() {
            var weapon = Instantiate(currentWeapon.WeaponPrefab, weaponHand);
            weapon.transform.localPosition = currentWeapon.Grip.position;
            weapon.transform.localRotation = currentWeapon.Grip.rotation;

            animator = GetComponentInChildren<Animator>();
            SetAttackAnimation();
        }

        public void MeleeAttack() {
            animator.SetTrigger("Attack");
            
            //TODO don't allow damaging until previous attack animation has finished
            foreach (var target in GetDamageablesInRange()) {
                target.TakeDamage(currentWeapon.Damage);
            }
        }

        private void SetAttackAnimation() {
            animator.runtimeAnimatorController = animOverride;
            animOverride["DEFAULT ATTACK"] = currentWeapon.AnimClip;
        }

        private List<IDamageable> GetDamageablesInRange() {
            var objectsInRange = Physics.OverlapSphere(transform.position, currentWeapon.Range);
            var damageables = new List<IDamageable>();
            foreach (var obj in objectsInRange) {
                //Don't deal damage to self
                if (obj.CompareTag("Player")) { continue; }

                var d = obj.gameObject.GetComponentInParent<IDamageable>();
                if (d != null) { damageables.Add(d); }
            }
            return damageables;
        }

        //private void OnDrawGizmos() {
        //   Gizmos.color = Color.green;
        //   Gizmos.DrawWireSphere(transform.position, currentWeapon.Range);
        //}
    }
}