using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Saving;

namespace RPG.Combat
{
    public class WeaponSystem : MonoBehaviour, ISaveable
    {
        [SerializeField] Weapon defaultWeapon = null;
        private List<Weapon> weapons = new List<Weapon>();
        private Weapon currentWeapon;
        private GameObject currentWeaponObj;

        [SerializeField] Transform leftHand = null;
        [SerializeField] Transform rightHand = null;

        public event Action<Weapon> onChangedWeapon;

        private void SetCurrentWeapon(Weapon newWeapon) {
            if (currentWeaponObj) { Destroy(currentWeaponObj); }
            onChangedWeapon(newWeapon);
        }


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

        #region SaveLoad
        public object SaveState() {
            var state = new SaveStateData();

            state.weapons = new List<string>();
            foreach (var w in weapons) { state.weapons.Add(w.name); }

            state.currentWeapon = currentWeapon.name;
            
            return state;
        }

        public void LoadState(object state) {
            var weaponState = (SaveStateData)state;
            var som = FindObjectOfType<ScriptableObjectManager>();

            weapons.Clear();
            foreach (var w in weaponState.weapons) {
                var weapon = som.GetWeapon(w);
                Assert.IsNotNull(weapon, w + " is not attached to the ScriptableObject Manager!");
                weapons.Add(weapon);
            }
            SetCurrentWeapon(som.GetWeapon(weaponState.currentWeapon));
        }

        [Serializable]
        private struct SaveStateData
        {
            public List<string> weapons;
            public string currentWeapon;
        }
        #endregion
    }
}