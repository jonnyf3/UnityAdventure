using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(menuName = "RPG/Weapon/Ranged")]
    public class RangedWeaponData : Weapon
    {
        [Header("Projectile")]
        public GameObject projectile;
        public Transform spawnPoint;
        public float launchSpeed = 10f;
        public GameObject endEffect;

        protected override void SetupBehaviour(GameObject obj) {
            var behaviour = obj.AddComponent<RangedWeapon>();
            behaviour.Data = this;
        }
    }
}