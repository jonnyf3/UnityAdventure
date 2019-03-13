using RPG.Combat;

namespace RPG.Actions
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        public override void Use() {
            PlaySoundEffect();
            DoParticleEffect();
            RestoreHealth();

            AbilityUsed();
        }

        private void DoParticleEffect() {
            var particles = Instantiate((Data as SelfHealData).healingParticles, gameObject.transform);
            var duration = particles.main.duration + particles.main.startLifetime.constant;
            Destroy(particles.gameObject, duration);
        }

        private void RestoreHealth() {
            var health = GetComponent<Health>();
            if (!health.IsDead) {
                health.RestoreHealth((Data as SelfHealData).healthRestored);
            }
        }
    }
}