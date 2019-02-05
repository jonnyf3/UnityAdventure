using UnityEngine;
using System.Collections.Generic;
using RPG.Weapons;
using RPG.Core;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animOverride = null;
        private Animator animator = null;

        [SerializeField] Transform weaponHand = null;

        //TODO make private, assign/extend via "unlocks"
        [SerializeField] List<Weapon> weapons = new List<Weapon>(0);
        private int currentWeaponIndex = 0;
        private Weapon CurrentWeapon {
            get {
                Assert.IsTrue(weapons.Count > 0, "Cannot get current weapon, no weapons assigned");
                return weapons[currentWeaponIndex];
            }
        }

        [SerializeField] List<MagicData> magicAbilities = new List<MagicData>(0);
        private int currentMagicIndex  = 0;
        private MagicData CurrentMagic {
            get {
                Assert.IsTrue(magicAbilities.Count > 0, "Cannot get current magic, no abilities assigned");
                return magicAbilities[currentMagicIndex];
            }
        }


        // Start is called before the first frame update
        void Start() {
            animator = GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animOverride;

            if (weapons.Count > 0) { EquipWeapon(weapons[0]); }
            if (magicAbilities.Count > 0) { EquipMagic(magicAbilities[0]); }
        }
        
        public void UseWeapon() {
            SetAttackAnimation(CurrentWeapon.AnimClip);
            animator.SetTrigger("Attack");
            
            //TODO don't allow damaging until previous attack animation has finished
            foreach (var target in GetDamageablesInRange()) {
                target.TakeDamage(CurrentWeapon.Damage);
            }
        }
        public void UseMagic() {
            var energy = GetComponent<Energy>();
            if (!energy.hasEnoughEnergy(CurrentMagic.EnergyCost)) {
                print("Insufficient energy!");
                return;
            }

            SetAttackAnimation(CurrentMagic.AnimClip);
            energy.UseEnergy(CurrentMagic.EnergyCost);
            CurrentMagic.Use();
        }
        
        public void CycleWeapon() {
            if (weapons.Count == 0) { return; }

            currentWeaponIndex = (currentWeaponIndex+1) % weapons.Count;
            EquipWeapon(CurrentWeapon);
        }
        public void CycleMagic() {
            if (magicAbilities.Count == 0) { return; }

            currentMagicIndex = (currentMagicIndex + 1) % magicAbilities.Count;
            EquipMagic(CurrentMagic);
        }

        private void EquipWeapon(Weapon weapon) {
            //TODO destroy previous weapon

            var weaponObj = Instantiate(weapon.WeaponPrefab, weaponHand);
            weaponObj.transform.localPosition = weapon.Grip.position;
            weaponObj.transform.localRotation = weapon.Grip.rotation;
        }
        private void EquipMagic(MagicData magic) {
            var currentMagic = GetComponent<IMagicBehaviour>();
            if (currentMagic != null) {
                Destroy((currentMagic as Component));
            }

            magic.AttachComponentTo(gameObject);
        }

        private void SetAttackAnimation(AnimationClip clip) {
            animOverride["DEFAULT ATTACK"] = clip;
        }

        private List<IDamageable> GetDamageablesInRange() {
            var objectsInRange = Physics.OverlapSphere(transform.position, CurrentWeapon.Range);
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