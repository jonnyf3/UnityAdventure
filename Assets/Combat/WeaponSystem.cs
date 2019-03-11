using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Combat
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] Weapon defaultWeapon = null;
        private List<Weapon> weapons = new List<Weapon>(0);

        [SerializeField] Transform leftHand = null;
        [SerializeField] Transform rightHand = null;

        private Weapon currentWeapon;
        private GameObject currentWeaponObj;

        private void SetCurrentWeapon(Weapon newWeapon) {
            if (currentWeaponObj) { Destroy(currentWeaponObj); }
            onChangedWeapon(newWeapon);
        }

        public delegate void OnChangedWeapon(Weapon newWeapon);
        public event OnChangedWeapon onChangedWeapon;

        private void Start() {
            Assert.IsNotNull(leftHand, "Must specify a left hand joint on " + gameObject);
            Assert.IsNotNull(rightHand, "Must specify a right hand joint on " + gameObject);

            onChangedWeapon += EquipWeapon;
            UnlockWeapon(defaultWeapon);
        }
        
        private void EquipWeapon(Weapon newWeapon) {
            currentWeapon = newWeapon;
            currentWeaponObj = currentWeapon.Setup(leftHand, rightHand);
        }

        public void UnlockWeapon(Weapon newWeapon) {
            weapons.Add(newWeapon);
            SetCurrentWeapon(newWeapon);
        }

        public void CycleWeapon(int step) {
            if (weapons.Count < 2) { return; }

            var currentIndex = weapons.IndexOf(currentWeapon);
            var newIndex = (int)Mathf.Repeat(currentIndex + step, weapons.Count);
            SetCurrentWeapon(weapons[newIndex]);
        }
    }
}