using UnityEngine;

namespace RPG.Weapons
{
    public class MagicProjectileBehaviour : MonoBehaviour, IMagicBehaviour
    {
        private MagicProjectileData data;
        public MagicProjectileData Data {
            set { data = value; }
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
            projectile.GetComponent<Rigidbody>().velocity = data.spawnPoint.forward * data.launchSpeed;
        }

        private GameObject CreateProjectile() {
            var projectile = Instantiate(data.projectile, data.spawnPoint.position, Quaternion.identity);

            //The created projectile needs to have its own behaviour
            var p = projectile.AddComponent<MagicProjectile>();
            p.Damage = data.damage;
            p.EndEffect = data.endEffect;

            return projectile;
        }
    }
}