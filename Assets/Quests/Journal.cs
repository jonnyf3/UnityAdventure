using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] List<Quest> quests = default;
        private GameObject activeObjectives;

        private void Start() {
            activeObjectives = new GameObject("Active Objectives");
            foreach (var quest in quests) {
                quest.Activate(activeObjectives);
                quest.onQuestCompleted += () => CompleteQuest(quest);
            }
        }

        private void CompleteQuest(Quest quest) {
            print("Completed quest!");
            quests.Remove(quest);
        }
    }
}