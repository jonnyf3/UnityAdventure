using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public abstract class Objective : MonoBehaviour
    {
        [Header("Objective")]
        [SerializeField] [TextArea(2, 8)] string description = "";
        public string Description => description;

        public delegate void OnObjectiveComplete(List<Objective> nextObjectives);
        public event OnObjectiveComplete onObjectiveComplete;

        public abstract void Activate();

        protected void CompleteObjective() {
            var nextObjectives = new List<Objective>();
            foreach (Transform objective in transform) {
                nextObjectives.Add(objective.GetComponent<Objective>());
            }
            onObjectiveComplete(nextObjectives);
            Destroy(this);
        }
    }
}