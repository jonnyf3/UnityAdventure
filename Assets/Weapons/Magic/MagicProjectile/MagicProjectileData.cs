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
        public GameObject endEffect = null;
        public float launchSpeed = 8f;
        public float damage = 50f;
        //this cannot be set on the scriptable object since it is not a prefab, it is part of the player
        public Transform spawnPoint { get; private set; }

        public override void AttachComponentTo(GameObject parent) {
            var behaviourComponent = parent.AddComponent<MagicProjectileBehaviour>();

            this.spawnPoint = parent.GetComponent<PlayerCombat>().SpawnPoint;
            Assert.IsNotNull(spawnPoint, "No spawn point provided for MagicProjectile ability");
            behaviourComponent.Data = this;

            behaviour = behaviourComponent;
        }
    }
}