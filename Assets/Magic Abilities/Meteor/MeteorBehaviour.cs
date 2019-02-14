using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RPG.Weapons;

namespace RPG.Magic
{
    public class MeteorBehaviour : MagicBehaviour
    {
        private MeteorData data = null;

        private void Start() {
            data = (Data as MeteorData);
        }

        public override void Use() {
            StartCoroutine(FireMeteor());
        }

        private IEnumerator FireMeteor() {
            var target = Instantiate(data.target, transform);

            //Hold down circle to increase range (up to max)
            while (Input.GetButton("Circle")) {
                float targetMoveDistance = data.targetMoveSpeed * Time.deltaTime;
                float newTargetPosition = Mathf.Clamp(target.transform.localPosition.z + targetMoveDistance, 0, data.maxRange);
                target.transform.localPosition = new Vector3(0, 0, newTargetPosition);
                yield return new WaitForEndOfFrame();
            }
            target.GetComponent<ParticleSystem>().Stop();
            var meteorStart = target.transform.GetChild(0);
            FireProjectile(meteorStart);

            //Notify Energy system that ability has been used
            AbilityUsed();

            float timeToImpact = Vector3.Distance(meteorStart.position, target.transform.position) / data.launchSpeed;
            yield return new WaitForSeconds(timeToImpact);
            foreach (var character in GetDamageablesInRange(target.transform.position, data.radius)) {
                //TODO damage fall off with distance from impact?
                character.TakeDamage(data.damage);
            }
            Destroy(target);
        }

        private void FireProjectile(Transform spawnPosition) {
            var meteor = Instantiate(data.projectile, spawnPosition.position, Quaternion.identity);
            var projectile = meteor.AddComponent<Projectile>();
            projectile.EndEffect = data.endEffect;

            meteor.GetComponent<Rigidbody>().velocity = spawnPosition.forward * data.launchSpeed;
        }
        
        private List<Health> GetDamageablesInRange(Vector3 centre, float range) {
            int mask = ~0;
            var objectsInRange = Physics.OverlapSphere(centre, range, mask, QueryTriggerInteraction.Ignore);

            var damageables = new List<Health>();
            foreach (var obj in objectsInRange) {
                //Don't deal damage to self
                if (obj == gameObject) { continue; }

                var d = obj.gameObject.GetComponentInParent<Health>();
                if (d != null) { damageables.Add(d); }
            }
            return damageables;
        }
    }
}