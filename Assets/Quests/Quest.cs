using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Quests
{
    public class Quest : MonoBehaviour
    {
        public delegate void OnQuestCompleted();
        public event OnQuestCompleted onQuestCompleted;

        public delegate void OnObjectiveUpdated(string objective);
        public event OnObjectiveUpdated onObjectiveUpdated;

        [SerializeField] string questName;
        [SerializeField][TextArea(3, 8)] string description;
        public string Name => questName;

        [SerializeField] int experiencePoints = 100;
        public int ExperiencePoints => experiencePoints;
        
        private List<Objective> objectives;
        private Objective activeObjective;
        public Objective ActiveObjective {
            get { return activeObjective; }
            set {
                activeObjective = value;
                activeObjective.onObjectiveComplete += ObjectiveComplete;
                onObjectiveUpdated(activeObjective.Description);
                activeObjective.Activate();
            }
        }

        public void StartQuest() {
            objectives = new List<Objective>();
            foreach (Transform objective in transform) {
                objectives.Add(objective.GetComponent<Objective>());
            }
            Assert.IsTrue(objectives.Count > 0, "Quest " + name + " has no objectives");

            //TODO allow multiple parallel active objectives?
            ActiveObjective = objectives[0];
        }

        private void ObjectiveComplete(List<Objective> nextObjectives) {
            objectives.Remove(activeObjective);
            foreach (var objective in nextObjectives) { objectives.Add(objective); }

            if (objectives.Count == 0) {
                onQuestCompleted();
                return;
            }
            ActiveObjective = objectives[0];
        }
    }
}