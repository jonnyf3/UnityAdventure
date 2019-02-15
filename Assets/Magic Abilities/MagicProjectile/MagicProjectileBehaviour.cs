using UnityEngine;
using RPG.Characters;
using RPG.Weapons;

namespace RPG.Magic
{
    public class MagicProjectileBehaviour : MagicBehaviour
    {
        private MagicProjectileData data = null;
        private Transform spawnPoint = null;

        private void Start() {
            data = (Data as MagicProjectileData);

            spawnPoint = CreateSpawnPoint();
            var player = GetComponent<PlayerController>();
            if (player) { player.SetMagicSpawnPoint(spawnPoint); }
        }

        public override void Use() {
            FireProjectile();
            AbilityUsed();
        }

        private Transform CreateSpawnPoint() {
            GameObject spawnObj = new GameObject("MagicProjectile Spawn");

            spawnObj.transform.parent = gameObject.transform;
            spawnObj.transform.localPosition = data.spawnPoint.position;
            spawnObj.transform.localRotation = Quaternion.identity;

            return spawnObj.transform;
        }

        private void FireProjectile() {
            var projectile = CreateProjectile();
            projectile.GetComponent<Rigidbody>().velocity = spawnPoint.forward * data.launchSpeed;
        }

        private GameObject CreateProjectile() {
            var projectile = Instantiate(data.projectile, spawnPoint.position, Quaternion.identity);

            //The created projectile needs to have its own behaviour
            var p = projectile.AddComponent<DamageProjectile>();
            p.Damage = data.damage;
            p.Owner = gameObject;
            p.EndEffect = data.endEffect;

            return projectile;
        }

        private void OnDestroy() {
            var player = GetComponent<PlayerController>();
            if (player) { player.SetMagicSpawnPoint(null); }

            Destroy(spawnPoint.gameObject);
        }
    }
}