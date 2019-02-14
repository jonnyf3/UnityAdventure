using UnityEngine;

namespace RPG.Weapons
{
    public class RangedWeapon : WeaponBehaviour
    {
        private Transform spawnPoint = null;
        private RangedWeaponData data = null;

        private void Start() {
            data = (Data as RangedWeaponData);

            CreateSpawnPoint();
        }

        public override void Attack() {
            FireProjectile();
        }

        private void FireProjectile() {
            var projectile = CreateProjectile();
            projectile.GetComponent<Rigidbody>().velocity = spawnPoint.forward * data.launchSpeed;
        }

        private GameObject CreateProjectile() {
            var projectile = Instantiate(data.projectile, spawnPoint.position, Quaternion.identity);

            //The created projectile needs to have its own behaviour to handle collisions
            var p = projectile.AddComponent<DamageProjectile>();
            p.Damage = data.Damage;
            p.Owner = owner;
            p.EndEffect = data.endEffect;

            return projectile;
        }

        private void CreateSpawnPoint() {
            GameObject spawnObj = new GameObject("Projectile Spawn");
            
            spawnObj.transform.parent = owner.transform;
            spawnObj.transform.localPosition = data.spawnPoint.position;
            spawnObj.transform.localRotation = Quaternion.identity;

            spawnPoint = spawnObj.transform;
        }
        
        private void OnDestroy() {
            Destroy(spawnPoint.gameObject);
        }
    }
}