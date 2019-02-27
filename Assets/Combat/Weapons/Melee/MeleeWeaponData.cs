using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(menuName = "RPG/Weapon/Melee")]
    public class MeleeWeaponData : WeaponData
    {
        protected override WeaponBehaviour GetWeaponBehaviour(GameObject weaponObj) {
            return weaponObj.AddComponent<MeleeWeapon>();
        }
    }
}