using UnityEngine;
using RPG.Characters;

namespace RPG.Combat
{
    public class RangedWeapon : WeaponBehaviour
    {
        private float Damage => Data.damage;
        private GameObject Projectile  => (Data as RangedWeaponData).projectile;
        private Transform  SpawnPoint  => (Data as RangedWeaponData).spawnPoint;
        private float      LaunchSpeed => (Data as RangedWeaponData).launchSpeed;
        private GameObject EndEffect   => (Data as RangedWeaponData).endEffect;

        private Transform spawnPoint = null;

        private void Start() {
            spawnPoint = CreateSpawnPoint();
            (owner as Player)?.SetRangedSpawnPoint(spawnPoint);
        }

        public override void Attack(Transform target = null) {
            if (owner as AICharacter) { AimAtTarget(target); }
            FireProjectile();
        }

        private void AimAtTarget(Transform target) {
            Vector3 aimOffset = Vector3.up * 1.25f;
            spawnPoint.forward = (target.position + aimOffset - spawnPoint.position).normalized;
        }
        private void FireProjectile() {
            var projectile = CreateProjectile();
            projectile.GetComponent<Rigidbody>().velocity = spawnPoint.forward * LaunchSpeed;
        }

        private GameObject CreateProjectile() {
            var projectile = Instantiate(Projectile, spawnPoint.position, Quaternion.identity);

            //The created projectile needs to have its own behaviour to handle collisions
            var p = projectile.AddComponent<DamageProjectile>();
            p.Owner = owner.gameObject;
            p.Damage = Damage;
            p.EndEffect = EndEffect;

            return projectile;
        }

        private Transform CreateSpawnPoint() {
            GameObject spawnObj = new GameObject("Projectile Spawn");
            
            spawnObj.transform.parent = owner.transform;
            spawnObj.transform.localPosition = SpawnPoint.position;
            spawnObj.transform.localRotation = Quaternion.identity;

            return spawnObj.transform;
        }
        
        private void OnDestroy() {
            (owner as Player)?.SetRangedSpawnPoint(null);

            Destroy(spawnPoint.gameObject);
        }
    }
}