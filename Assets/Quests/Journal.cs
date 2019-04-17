using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour, ISaveable
    {
        [SerializeField] List<Quest> quests = default;
        private QuestManager qm;

        public event Action<string, List<string>> onQuestChanged;

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
            qm = FindObjectOfType<QuestManager>();
            if (quests.Count > 0) {
                foreach (var quest in quests) { StartQuest(quest); }
                ActiveQuest = quests[0];
            } else {
                ActiveQuest = null;
            }
        }

        public void StartQuest(Quest quest) {
            quest.Reset(qm.gameObject);

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

        #region SaveLoad
        public object SaveState() {
            var state = new SaveStateData();

            state.quests = new Dictionary<string, List<int>>();
            foreach (var q in quests) {
                state.quests[q.name] = new List<int>();
                foreach (var o in q.IncompleteObjectives) {
                    state.quests[q.name].Add(o);
                }
            }
            state.activeQuest = ActiveQuest.name;

            return state;
        }

        public void LoadState(object state) {
            var questState = (SaveStateData)state;

            foreach (var ob in qm.GetComponents<ObjectiveBehaviour>()) { Destroy(ob); }

            quests.Clear();
            foreach (var q in questState.quests) {
                var quest = qm.GetQuest(q.Key);
                StartQuest(quest);

                var remainingObjectives = q.Value;
                foreach (var o in quest.Objectives) {
                    if (!remainingObjectives.Contains(o.Key)) {
                        o.Value.Complete();
                    }
                }
            }

            ActiveQuest = qm.GetQuest(questState.activeQuest);
        }

        [Serializable]
        private struct SaveStateData
        {
            public Dictionary<string, List<int>> quests;
            public string activeQuest;
        }
        #endregion
    }
}