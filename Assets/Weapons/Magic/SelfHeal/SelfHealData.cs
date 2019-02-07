using UnityEngine;

namespace RPG.Magic
{
    [CreateAssetMenu(menuName = "RPG/Magic/SelfHeal")]
    public class SelfHealData : MagicData
    {
        [Header("Self-Healing")]
        public float healthRestored = 5f;

        protected override MagicBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<SelfHealBehaviour>();
        }
    }
}