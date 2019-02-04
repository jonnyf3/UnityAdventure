using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Magic/SelfHeal")]
    public class SelfHealData : MagicData
    {
        [Header("Self-Healing")]
        public ParticleSystem effect = null;
        public float healthRestored = 5f;

        public override void AttachComponentTo(GameObject parent) {
            var behaviourComponent = parent.AddComponent<SelfHealBehaviour>();
            behaviourComponent.Data = this;

            behaviour = behaviourComponent;
        }
    }
}