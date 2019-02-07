﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using RPG.Core;
using RPG.Weapons;
using RPG.Magic;

namespace RPG.Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animOverride = null;
        private Animator animator = null;

        [SerializeField] Transform weaponHand = null;
        private GameObject currentWeaponObject = null;

        //TODO make private, assign/extend via "unlocks"
        [SerializeField] List<Weapon> weapons = new List<Weapon>(0);
        private int currentWeaponIndex = 0;
        private Weapon CurrentWeapon {
            get {
                Assert.IsTrue(weapons.Count > 0, "Cannot get current weapon, no weapons assigned");
                return weapons[currentWeaponIndex];
            }
        }
        public delegate void OnChangedWeapon(Weapon weapon);
        public event OnChangedWeapon onChangedWeapon = null;

        [SerializeField] List<MagicData> magicAbilities = new List<MagicData>(0);
        private int currentMagicIndex  = 0;
        private MagicData CurrentMagic {
            get {
                Assert.IsTrue(magicAbilities.Count > 0, "Cannot get current magic, no abilities assigned");
                return magicAbilities[currentMagicIndex];
            }
        }
        public delegate void OnChangedMagic(MagicData magic);
        public event OnChangedMagic onChangedMagic = null;


        // Start is called before the first frame update
        void Start() {
            animator = GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animOverride;

            onChangedWeapon += EquipWeapon;
            onChangedMagic  += EquipMagic;

            if (weapons.Count > 0) { onChangedWeapon(weapons[0]); }
            if (magicAbilities.Count > 0) { onChangedMagic(magicAbilities[0]); }
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
        
        public void CycleWeapon(int step) {
            if (weapons.Count == 0) { return; }

            currentWeaponIndex += step;
            if (currentWeaponIndex < 0) { currentWeaponIndex += magicAbilities.Count; }
            currentWeaponIndex %= weapons.Count;
            onChangedWeapon(CurrentWeapon);
        }
        public void CycleMagic(int step) {
            if (magicAbilities.Count == 0) { return; }
            
            currentMagicIndex += step;
            if (currentMagicIndex < 0) { currentMagicIndex += magicAbilities.Count; }
            currentMagicIndex %= magicAbilities.Count;
            onChangedMagic(CurrentMagic);
        }


        public void AddWeapon(Weapon newWeapon) {
            weapons.Add(newWeapon);
            currentWeaponIndex = weapons.Count - 1;
            onChangedWeapon(newWeapon);
        }

        private void EquipWeapon(Weapon weapon) {
            Destroy(currentWeaponObject);

            currentWeaponObject = Instantiate(weapon.WeaponPrefab, weaponHand);
            currentWeaponObject.transform.localPosition = weapon.Grip.position;
            currentWeaponObject.transform.localRotation = weapon.Grip.rotation;
        }
        private void EquipMagic(MagicData magic) {
            var currentMagic = GetComponent<MagicBehaviour>();
            if (currentMagic != null) {
                Destroy((currentMagic as Component));
            }

            magic.AttachBehaviourTo(gameObject);
        }

        private void SetAttackAnimation(AnimationClip clip) {
            animOverride["DEFAULT ATTACK"] = clip;
        }

        private List<IDamageable> GetDamageablesInRange() {
            var objectsInRange = Physics.OverlapSphere(transform.position, CurrentWeapon.Range);
            var damageables = new List<IDamageable>();
            foreach (var obj in objectsInRange) {
                //Don't deal damage to self
                //TODO could this be done by obj == gameObject to be more general?
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