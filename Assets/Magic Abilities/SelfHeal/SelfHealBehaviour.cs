using RPG.Characters;

namespace RPG.Magic
{
    public class SelfHealBehaviour : MagicBehaviour
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