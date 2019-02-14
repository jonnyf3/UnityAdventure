using UnityEngine;

namespace RPG.Magic
{
    [CreateAssetMenu(menuName = "RPG/Magic/Meteor")]
    public class MeteorData : MagicData
    {
        [Header("Impact")]
        public GameObject target = null;
        public float radius = 5f;
        public float maxRange = 20f;

        [Header("Meteor")]
        public GameObject projectile = null;
        public float launchSpeed = 8f;
        public float damage = 100f;
        public GameObject endEffect = null;

        protected override MagicBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<MeteorBehaviour>();
        }
    }
}