using UnityEngine;
using RPG.Weapons;

namespace RPG.Magic
{
    public class MagicProjectileBehaviour : MagicBehaviour
    {
        private MagicProjectileData data = null;
        private Transform spawnPoint = null;

        private void Start() {
            data = (Data as MagicProjectileData);

            CreateSpawnPoint();
        }

        public override void Use() {
            FireProjectile();
            AbilityUsed();
        }

        private void CreateSpawnPoint() {
            GameObject spawnObj = new GameObject("MagicProjectile Spawn");

            spawnObj.transform.parent = gameObject.transform;
            spawnObj.transform.localPosition = data.spawnPoint.position;
            spawnObj.transform.localRotation = Quaternion.identity;

            spawnPoint = spawnObj.transform;
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
            Destroy(spawnPoint.gameObject);
        }
    }
}