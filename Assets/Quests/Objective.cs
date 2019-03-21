using UnityEngine;

namespace RPG.Quests
{
    public abstract class Objective : ScriptableObject
    {
        public delegate void OnObjectiveComplete();
        public event OnObjectiveComplete onObjectiveComplete;

        public abstract void Activate();
        protected void CompleteObjective() => onObjectiveComplete();

        [SerializeField] [TextArea(3, 8)] string description = "";
        public string Description => description;
    }
}