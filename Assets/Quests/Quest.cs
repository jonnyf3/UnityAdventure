using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = "RPG/Quests/Quest")]
    public class Quest : ScriptableObject
    {
        public delegate void OnQuestCompleted();
        public event OnQuestCompleted onQuestCompleted;

        public delegate void OnObjectiveUpdated(string objective);
        public event OnObjectiveUpdated onObjectiveUpdated;

        [SerializeField] int experiencePoints = 100;
        public int ExperiencePoints => experiencePoints;

        [SerializeField] List<Objective> objectives = default;
        private List<Objective> instanceObjectives;

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
            Assert.IsTrue(objectives.Count > 0, "Quest " + name + " has no objectives");
            instanceObjectives = new List<Objective>(objectives);
            //TODO allow multiple parallel active objectives?
            ActiveObjective = instanceObjectives[0];
        }

        private void ObjectiveComplete() {
            instanceObjectives.Remove(activeObjective);
            if (instanceObjectives.Count == 0) {
                onQuestCompleted();
                return;
            }
            ActiveObjective = instanceObjectives[0];
        }
    }
}