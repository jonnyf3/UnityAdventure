using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Weapon/Ranged")]
    public class RangedWeaponData : WeaponData
    {
        [Header("Ranged Weapon")]
        public GameObject projectile;
        public Transform spawnPoint;
        public float launchSpeed = 10f;
        public GameObject endEffect;

        protected override WeaponBehaviour GetWeaponBehaviour(GameObject weaponObj) {
            return weaponObj.AddComponent<RangedWeapon>();
        }
    }
}