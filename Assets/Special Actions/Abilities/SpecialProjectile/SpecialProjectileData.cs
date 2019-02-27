using UnityEngine;

namespace RPG.Actions
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Special Projectile")]
    public class SpecialProjectileData : AbilityData
    {
        [Header("Projectile")]
        public GameObject projectile = null;
        public Transform spawnPoint = null;
        public float launchSpeed = 8f;
        public float damage = 50f;
        public GameObject endEffect = null;

        protected override AbilityBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<SpecialProjectileBehaviour>();
        }
    }
}