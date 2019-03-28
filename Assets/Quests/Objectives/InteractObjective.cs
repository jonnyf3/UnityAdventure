using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class InteractObjective : Objective
    {
        public string Target = "";

        public InteractObjective(Vector2 position) : base(position) { }

        protected override ObjectiveBehaviour AddBehaviour(GameObject objectiveTracker) {
            return objectiveTracker.AddComponent<InteractObjectiveBehaviour>();
        }
    }
}