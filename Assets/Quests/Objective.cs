using System;
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

        public event Action onStarted;
        public event Action onCompleted;
        public void Start()    => onStarted();
        public void Complete() => onCompleted();
        
        private List<Objective> prerequisites = new List<Objective>();
        public bool CanStart => prerequisites.Count == 0;

        public Objective(Vector2 position) {
            nodePosition = position;
            id = -1;
        }

        public void Reset(GameObject objectiveTracker) {
            prerequisites = new List<Objective>();
            onStarted = null;
            onCompleted = null;

            onStarted += () => {
                var objectiveBehaviour = AddBehaviour(objectiveTracker);
                objectiveBehaviour.Setup(this);
            };
        }
        protected abstract ObjectiveBehaviour AddBehaviour(GameObject objectiveTracker);

        public void AddPrerequisite(Objective objective) {
            if (!prerequisites.Contains(objective)) {
                prerequisites.Add(objective);
                objective.onCompleted += () => CompletePrerequisite(objective);
            }
        }
        private void CompletePrerequisite(Objective objective) {
            prerequisites.Remove(objective);
            if (CanStart) { Start(); }
        }
    }
}