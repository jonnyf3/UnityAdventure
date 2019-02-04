using UnityEngine;
using RPG.Characters;

namespace RPG.Weapons
{
    public class SelfHealBehaviour : MonoBehaviour, IMagicBehaviour
    {
        private SelfHealData data;
        public SelfHealData Data {
            set { data = value; }
        }

        public void Use() {
            var health = GetComponent<Health>();
            health.TakeDamage(-data.healthRestored);

            DoAnimation();
            DoParticleEffect();
        }

        private void DoAnimation() {
            //TODO this behaviour isn't really an "attack", but still using the Attack animation state
            var animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Attack");
        }

        private void DoParticleEffect() {
            var particles = Instantiate(data.effect, gameObject.transform);
            //TODO destroy particles after effect completion
        }
    }
}