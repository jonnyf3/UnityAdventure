using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using RPG.Weapons;

namespace RPG.Characters
{
    public class PlayerCombat : MonoBehaviour
    {
        private Character character;

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

        // Start is called before the first frame update
        void Start() {
            character = GetComponent<Character>();

            onChangedWeapon += EquipWeapon;

            if (weapons.Count > 0) { onChangedWeapon(weapons[0]); }
        }
        
        public void UseWeapon() {
            character.DoCustomAnimation(CurrentWeapon.AnimClip);
            
            //TODO don't allow damaging until previous attack animation has finished
            foreach (var target in GetDamageablesInRange()) {
                target.TakeDamage(CurrentWeapon.Damage);
            }
        }
        
        public void CycleWeapon(int step) {
            if (weapons.Count == 0) { return; }

            currentWeaponIndex += step;
            if (currentWeaponIndex < 0) { currentWeaponIndex += weapons.Count; }
            currentWeaponIndex %= weapons.Count;
            onChangedWeapon(CurrentWeapon);
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

        private List<Health> GetDamageablesInRange() {
            int mask = ~0;
            var objectsInRange = Physics.OverlapSphere(transform.position, CurrentWeapon.Range, mask, QueryTriggerInteraction.Ignore);

            var damageables = new List<Health>();
            foreach (var obj in objectsInRange) {
                //Don't deal damage to self
                //TODO could this be done by obj == gameObject to be more general?
                if (obj.CompareTag("Player")) { continue; }

                var d = obj.gameObject.GetComponentInParent<Health>();
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