using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] List<Quest> activeQuests = default;
        private Quest currentQuest;

        public delegate void OnQuestUpdated(string questText, string objectiveText);
        public event OnQuestUpdated onQuestUpdated;

        void Start() {
            if (activeQuests.Count > 0) {
                foreach(var quest in activeQuests){
                    quest.onObjectiveUpdated += (objective) => onQuestUpdated(quest.Name, objective);
                    quest.onQuestCompleted += () => CompleteQuest(quest);
                    quest.StartQuest();
                }
                currentQuest = activeQuests[0];
            }
        }

        private void CompleteQuest(Quest quest) {
            print("Completed quest! Received " + quest.ExperiencePoints + " XP");
            activeQuests.Remove(quest);
            if (activeQuests.Count == 0) {
                onQuestUpdated("", "");
                return;
            }
            currentQuest = activeQuests[0];
            onQuestUpdated(currentQuest.Name, currentQuest.ActiveObjective.Description);
        }
    }
}