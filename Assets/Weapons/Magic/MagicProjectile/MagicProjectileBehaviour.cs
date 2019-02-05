using UnityEngine;

namespace RPG.Weapons
{
    public class MagicProjectileBehaviour : MonoBehaviour, IMagicBehaviour
    {
        private MagicProjectileData data;
        public MagicProjectileData Data {
            set { data = value; }
        }

        private Transform spawnPoint = null;

        private void Start() {
            GameObject spawnObj = new GameObject("MagicProjectile Spawn");
            //Create spawn object as child of body (to ensure correct relative forward)
            var body = GetComponentInChildren<Animator>().gameObject;
            spawnObj.transform.parent = body.transform;
            //Set local position to be correct based on specified prefab transform
            spawnObj.transform.localPosition = data.spawnPoint.position;
            spawnPoint = spawnObj.transform;
        }

        //Implement IMagic interface
        public void Use() {
            DoAttackAnimation();
            FireProjectile();
        }

        private void DoAttackAnimation() {
            var animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Attack");
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