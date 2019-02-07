using UnityEngine;

namespace RPG.Magic
{
    public class MagicProjectileBehaviour : MagicBehaviour
    {
        private Transform spawnPoint = null;
        private MagicProjectileData data = null;

        private void Start() {
            data = (Data as MagicProjectileData);

            CreateSpawnPoint();
        }

        public override void Use() {
            DoAnimation();
            FireProjectile();
        }

        private void CreateSpawnPoint() {
            GameObject spawnObj = new GameObject("MagicProjectile Spawn");
            
            //Create spawn object as child of body (to ensure correct relative forward)
            var body = GetComponentInChildren<Animator>().gameObject;
            spawnObj.transform.parent = body.transform;
            
            //Set local position based on specified prefab transform
            spawnObj.transform.localPosition = data.spawnPoint.position;
            spawnPoint = spawnObj.transform;
        }

        private void FireProjectile() {
            var projectile = CreateProjectile();
            projectile.GetComponent<Rigidbody>().velocity = spawnPoint.forward * data.launchSpeed;
        }

        private GameObject CreateProjectile() {
            var projectile = Instantiate(data.projectile, spawnPoint.position, Quaternion.identity);

            //The created projectile needs to have its own behaviour
            var p = projectile.AddComponent<MagicProjectile>();
            p.Damage = data.damage;
            p.EndEffect = data.endEffect;

            return projectile;
        }

        private void OnDestroy() {
            Destroy(spawnPoint.gameObject);
        }
    }
}