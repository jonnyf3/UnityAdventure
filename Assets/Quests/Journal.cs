using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] List<Quest> quests = default;
        private GameObject activeObjectives;

        public delegate void OnQuestChanged(string name, List<string> objectives);
        public event OnQuestChanged onQuestChanged;

        private Quest activeQuest;
        private Quest ActiveQuest {
            get { return activeQuest; }
            set {
                activeQuest = value;
                if (activeQuest == null) {
                    onQuestChanged("", new List<string>());
                } else {
                    onQuestChanged(activeQuest.name, activeQuest.ActiveObjectives);
                }
            }
        }

        private void Start() {
            activeObjectives = new GameObject("Active Objectives");
            if (quests.Count > 0) {
                foreach (var quest in quests) { StartQuest(quest); }
                ActiveQuest = quests[0];
            } else {
                ActiveQuest = null;
            }
        }

        public void StartQuest(Quest quest) {
            quest.Reset(activeObjectives);

            if (!quests.Contains(quest)) { quests.Add(quest); }
            quest.onQuestChanged += () => ActiveQuest = quest;
            quest.onQuestCompleted += () => CompleteQuest(quest);

            quest.Start();
        }

        private void CompleteQuest(Quest quest) {
            print("Completed quest! Received " + quest.experiencePoints + "XP!");
            quests.Remove(quest);
            if (quests.Count == 0) {
                ActiveQuest = null;
                return;
            }
            ActiveQuest = quests[0];
        }
    }
}