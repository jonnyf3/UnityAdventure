using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animOverride = null;
        private Animator animator = null;

        [SerializeField] Transform weaponHand = null;
        [SerializeField] Weapon currentWeapon = null;

        [SerializeField] MagicData[] magicAbilities = new MagicData[0];

        // Start is called before the first frame update
        void Start() {
            var weapon = Instantiate(currentWeapon.WeaponPrefab, weaponHand);
            weapon.transform.localPosition = currentWeapon.Grip.position;
            weapon.transform.localRotation = currentWeapon.Grip.rotation;

            animator = GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animOverride;
            
            Assert.IsTrue(magicAbilities.Length > 0, "No magic abilities assigned");
            magicAbilities[0].AttachComponentTo(gameObject);
        }

        private void SetAttackAnimation(AnimationClip clip) {
            animOverride["DEFAULT ATTACK"] = clip;
        }

        public void MeleeAttack() {
            SetAttackAnimation(currentWeapon.AnimClip);
            animator.SetTrigger("Attack");
            
            //TODO don't allow damaging until previous attack animation has finished
            foreach (var target in GetDamageablesInRange()) {
                target.TakeDamage(currentWeapon.Damage);
            }
        }

        public void UseSpecialAbility(int abilityIndex) {
            var energy = GetComponent<Energy>();
            var ability = magicAbilities[abilityIndex];

            if (!energy.hasEnoughEnergy(ability.EnergyCost)) {
                print("Insufficient energy!");
                return;
            }

            SetAttackAnimation(ability.AnimClip);
            energy.UseEnergy(ability.EnergyCost);
            
            ability.Use();
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