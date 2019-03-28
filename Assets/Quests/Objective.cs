using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public abstract class Objective
    {
        //Objective data
        public string description;
        
        //Data which needs to be serialized and saved in the asset
        public int id;
        public Vector2 nodePosition;

        public delegate void OnStarted();
        public event OnStarted onStarted;
        
        public delegate void OnCompleted();
        public event OnCompleted onCompleted;
        public void Complete() => onCompleted();
        
        public Objective(Vector2 position) {
            nodePosition = position;
            prerequisites = new List<Objective>();
            id = -1;
        }

        private List<Objective> prerequisites = new List<Objective>();
        public void AddPrerequisite(Objective objective) {
            if (prerequisites == null) { prerequisites = new List<Objective>(); }
            if (!prerequisites.Contains(objective)) {
                prerequisites.Add(objective);
                objective.onCompleted += () => CompletePrerequisite(objective);
            }
        }
        private void CompletePrerequisite(Objective objective) {
            prerequisites.Remove(objective);
            TryStart();
        }

        public void Activate(GameObject objectiveTracker) {
            onStarted = () => {
                var objectiveBehaviour = AddBehaviour(objectiveTracker);
                objectiveBehaviour.Setup(this);
            };
        }
        protected abstract ObjectiveBehaviour AddBehaviour(GameObject objectiveTracker);

        public void TryStart() {
            if (prerequisites == null || prerequisites.Count == 0) { onStarted(); }
        }
    }
}