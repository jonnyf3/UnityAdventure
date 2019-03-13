using UnityEngine;

namespace RPG.Actions
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/SelfHeal")]
    public class SelfHealData : AbilityData
    {
        [Header("Self-Healing")]
        public float healthRestored = 5f;
        public ParticleSystem healingParticles = default;

        protected override AbilityBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<SelfHealBehaviour>();
        }
    }
}