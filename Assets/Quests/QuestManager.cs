using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] List<Quest> quests;

        public Quest GetQuest(string name) {
            return quests.Find((quest) => quest.name == name);
        }
    }
}