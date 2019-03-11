using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(menuName = "RPG/Weapon/Melee")]
    public class MeleeWeaponData : Weapon
    {
        protected override void SetupBehaviour(GameObject obj) {
            var behaviour = obj.AddComponent<MeleeWeapon>();
            behaviour.Data = this;
        }
    }
}