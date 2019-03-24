using UnityEngine;

namespace RPG.Quests
{
    public class Objective
    {
        public string description;

        public delegate void OnObjectiveCompleted();
        public event OnObjectiveCompleted onObjectiveCompleted;
        public void Complete() => onObjectiveCompleted();

        public Vector2 position { get; set; }

        public Objective(Vector2 position) {
            this.position = position;
        }
    }
}