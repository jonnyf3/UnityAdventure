using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] List<Quest> quests = default;
        private Quest activeQuest;
        private GameObject activeObjectives;

        private void Start() {
            activeObjectives = new GameObject("Active Objectives");
            if (quests.Count == 0) { return; }

            foreach (var quest in quests) { StartQuest(quest); }
            activeQuest = quests[0];
        }

        public void StartQuest(Quest quest) {
            if (!quests.Contains(quest)) { quests.Add(quest); }
            quest.Activate(activeObjectives);
            quest.onQuestCompleted += () => CompleteQuest(quest);
            activeQuest = quest;
        }

        private void CompleteQuest(Quest quest) {
            print("Completed quest! Received " + quest.experiencePoints + "XP!");
            quests.Remove(quest);
            if (quests.Count == 0) {
                activeQuest = null;
                return;
            }
            activeQuest = quests[0];
        }
    }
}