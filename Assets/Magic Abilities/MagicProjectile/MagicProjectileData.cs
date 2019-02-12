using UnityEngine;

namespace RPG.Magic
{
    [CreateAssetMenu(menuName = "RPG/Magic/Magic Projectile")]
    public class MagicProjectileData : MagicData
    {
        [Header("Projectile")]
        public GameObject projectile = null;
        public Transform spawnPoint = null;
        public float launchSpeed = 8f;
        public float damage = 50f;
        public GameObject endEffect = null;

        protected override MagicBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<MagicProjectileBehaviour>();
        }
    }
}