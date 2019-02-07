using RPG.Characters;

namespace RPG.Magic
{
    public class SelfHealBehaviour : MagicBehaviour
    {
        public override void Use() {
            DoAnimation();
            RestoreHealth();
            DoParticleEffect();
        }

        private void RestoreHealth() {
            var health = GetComponent<Health>();
            health.RestoreHealth((Data as SelfHealData).healthRestored);
        }
    }
}