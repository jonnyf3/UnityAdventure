using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.Weapons
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

        public override void AttachComponentTo(GameObject parent) {
            var behaviourComponent = parent.AddComponent<MagicProjectileBehaviour>();
            behaviourComponent.Data = this;

            behaviour = behaviourComponent;
        }
    }
}