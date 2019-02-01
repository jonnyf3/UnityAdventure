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

        [SerializeField] Transform weaponHand = null;
        [SerializeField] Transform magicSpawn = null;
        [SerializeField] Weapon currentWeapon = null;
        [SerializeField] Magic currentMagic = null;
        private Energy energy = null;

        // Start is called before the first frame update
        void Start() {
            var weapon = Instantiate(currentWeapon.WeaponPrefab, weaponHand);
            weapon.transform.localPosition = currentWeapon.Grip.position;
            weapon.transform.localRotation = currentWeapon.Grip.rotation;

            animator = GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animOverride;

            energy = GetComponent<Energy>();
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

        public void MagicAttack() {
            if (!energy.hasEnoughEnergy(currentMagic.EnergyCost)) {
                print("Insufficient energy!");
                return;
            }
            
            SetAttackAnimation(currentMagic.AnimClip);
            animator.SetTrigger("Attack");

            CreateMagic();
            energy.UseEnergy(currentMagic.EnergyCost);
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

        private void CreateMagic() {
            var magic = Instantiate(currentMagic.Effect, magicSpawn.position, Quaternion.identity);

            var projectile = magic.GetComponent<MagicProjectile>();
            if (projectile) {
                projectile.Damage = currentMagic.Damage;
                magic.GetComponent<Rigidbody>().velocity = magicSpawn.forward * projectile.LaunchSpeed;
            }
        }
        
        //private void OnDrawGizmos() {
        //   Gizmos.color = Color.green;
        //   Gizmos.DrawWireSphere(transform.position, currentWeapon.Range);
        //}
    }
}