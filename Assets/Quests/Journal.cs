using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] List<Quest> activeQuests = default;

        void Start() {
            foreach(var quest in activeQuests){
                quest.onObjectiveUpdated += (text) => print(text);
                quest.onQuestCompleted += () => CompleteQuest(quest);
                quest.StartQuest();
            }
        }

        private void CompleteQuest(Quest quest) {
            print("Completed quest! Received " + quest.ExperiencePoints + " XP");
            activeQuests.Remove(quest);
        }
    }
}