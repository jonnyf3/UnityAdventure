using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Weapon/Melee")]
    public class MeleeWeaponData : WeaponData
    {
        protected override WeaponBehaviour GetWeaponBehaviour(GameObject weaponObj) {
            return weaponObj.AddComponent<MeleeWeapon>();
        }
    }
}