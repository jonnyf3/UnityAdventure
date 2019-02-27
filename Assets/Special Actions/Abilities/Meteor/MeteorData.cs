using UnityEngine;

namespace RPG.Actions
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Meteor")]
    public class MeteorData : AbilityData
    {
        [Header("Impact")]
        public GameObject target = null;
        public float radius = 5f;
        public float maxRange = 20f;
        public float targetMoveSpeed = 5f;

        [Header("Meteor")]
        public GameObject projectile = null;
        public float launchSpeed = 8f;
        public float damage = 100f;
        public GameObject endEffect = null;

        protected override AbilityBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<MeteorBehaviour>();
        }
    }
}