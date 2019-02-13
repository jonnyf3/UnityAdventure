using System.Collections.Generic;
using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        private Character character;

        [SerializeField] WeaponData defaultWeapon = null;
        private List<WeaponData> weapons = new List<WeaponData>(0);

        [SerializeField] Transform weaponHand = null;
        private GameObject currentWeaponObject;

        private WeaponData currentWeapon;
        public WeaponData CurrentWeapon {
            get { return currentWeapon; }
            private set {
                currentWeapon = value;
                EquipWeapon();
            }
        }
        public delegate void OnChangedWeapon(WeaponData weapon);
        public event OnChangedWeapon onChangedWeapon = null;

        private void Start() {
            character = GetComponent<Character>();

            UnlockWeapon(defaultWeapon);
        }

        public void Attack() {
            character.DoCustomAnimation(CurrentWeapon.AnimClip);
            CurrentWeapon.Attack();
        }

        public void CycleWeapon(int step) {
            if (weapons.Count == 0) { return; }

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

            currentWeaponObject = Instantiate(CurrentWeapon.WeaponPrefab, weaponHand);
            CurrentWeapon.SetupWeapon(currentWeaponObject, gameObject);

            onChangedWeapon?.Invoke(CurrentWeapon);
        }
    }
}