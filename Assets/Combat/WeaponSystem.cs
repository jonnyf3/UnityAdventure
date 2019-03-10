using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Combat
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] WeaponData defaultWeapon = null;
        private List<WeaponData> weapons = new List<WeaponData>(0);

        [SerializeField] Transform leftHand = null;
        [SerializeField] Transform rightHand = null;
        private GameObject currentWeaponObject;

        private WeaponData currentWeapon;
        private WeaponData CurrentWeapon {
            get { return currentWeapon; }
            set {
                currentWeapon = value;
                EquipWeapon();
            }
        }
        public delegate void OnChangedWeapon(WeaponData newWeapon);
        public event OnChangedWeapon onChangedWeapon;

        private void Start() {
            Assert.IsNotNull(leftHand, "Must specify a left hand joint on " + gameObject);
            Assert.IsNotNull(rightHand, "Must specify a right hand joint on " + gameObject);

            UnlockWeapon(defaultWeapon);
        }

        public void CycleWeapon(int step) {
            if (weapons.Count < 2) { return; }

            var currentIndex = weapons.IndexOf(CurrentWeapon);
            var newIndex = (int)Mathf.Repeat(currentIndex + step, weapons.Count);
            CurrentWeapon = weapons[newIndex];
        }

        public void UnlockWeapon(WeaponData newWeapon) {
            weapons.Add(newWeapon);
            CurrentWeapon = newWeapon;
        }

        private void EquipWeapon() {
            Destroy(currentWeaponObject);

            Transform hand = null;
            if (CurrentWeapon.PreferredHand == WeaponData.Handedness.Left)  { hand = leftHand; }
            if (CurrentWeapon.PreferredHand == WeaponData.Handedness.Right) { hand = rightHand; }
            Assert.IsNotNull(hand, "Must specify a preferred hand for the current weapon!");

            currentWeaponObject = CurrentWeapon.SetupWeaponOnCharacter(gameObject, hand);

            onChangedWeapon(CurrentWeapon);
        }

        public void DoWeaponBehaviour() {
            currentWeaponObject.GetComponent<WeaponBehaviour>().Attack();
        }
    }
}