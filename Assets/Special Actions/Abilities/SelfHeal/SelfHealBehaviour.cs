using RPG.Combat;

namespace RPG.Actions
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        public override void Use() {
            DoParticleEffect();
            PlaySoundEffect();
            RestoreHealth();
            AbilityUsed();
        }

        private void RestoreHealth() {
            var health = GetComponent<Health>();
            if (!health.IsDead) {
                health.RestoreHealth((Data as SelfHealData).healthRestored);
            }
        }
    }
}