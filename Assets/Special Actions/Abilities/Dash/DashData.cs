using UnityEngine;

namespace RPG.Actions
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Dash")]
    public class DashData : AbilityData
    {
        [Header("Dash")]
        public float range = 5f;
        public float dashSpeed = 10f;
        public GameObject prefab = default;
        public Material dashMaterial = default;
        public Material badMaterial = default;

        protected override AbilityBehaviour GetBehaviourComponent(GameObject parent) {
            return parent.AddComponent<DashBehaviour>();
        }
    }
}