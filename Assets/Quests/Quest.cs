using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = "RPG/Quest")]
    public class Quest : ScriptableObject
    {
        public string questName;
        public int experiencePoints;

        //Serialization (to save Quest SO as an asset) does not support inheritance
        //Need a separate list of each base objective type in order to properly store their data
        //Combine these lists together as a property at run-time, rather than saving it
        [SerializeField] List<KillObjective>   killObjectives   = new List<KillObjective>();
        [SerializeField] List<TravelObjective> travelObjectives = new List<TravelObjective>();
        public Dictionary<int, Objective> Objectives {
            get {
                var objectives = new Dictionary<int, Objective>();
                foreach (var k in killObjectives)   { objectives.Add(k.id, k); }
                foreach (var t in travelObjectives) { objectives.Add(t.id, t); }
                return objectives;
            }
        }

        #region EditQuest
        public delegate void OnChanged();
        public event OnChanged onChanged;

        public void AddObjective(Objective objective) {
            Undo.RecordObject(this, "Add objective");

            objective.id = GetNextObjectiveID();
            if (objective as KillObjective != null) {
                killObjectives.Add(objective as KillObjective);
            }
            else if (objective as TravelObjective != null) {
                travelObjectives.Add(objective as TravelObjective);
            }
            onChanged();
        }
        public void Delete(Objective objective) {
            Undo.RecordObject(this, "Delete objective");

            BreakLinks(objective);
            if (objective as KillObjective != null) {
                killObjectives.Remove(objective as KillObjective);
            }
            if (objective as TravelObjective != null) {
                travelObjectives.Remove(objective as TravelObjective);
            }
            onChanged();
        }
        public int GetNextObjectiveID() {
            for (int i = 0; i < Objectives.Count; i++) {
                if (!Objectives.ContainsKey(i)) { return i; }
            }
            return Objectives.Count;
        }
        
        public List<Link> dependencies = new List<Link>();
        public void AddLink(Objective o1, Objective o2) {
            dependencies.Add(new Link(o1.id, o2.id));
            onChanged();
        }
        public void BreakLinks(Objective o) {
            var linksToRemove = new List<Link>();
            foreach (var link in dependencies) {
                if (link.parentID == o.id || link.childID == o.id) {
                    linksToRemove.Add(link);
                }
            }
            foreach (var link in linksToRemove) { dependencies.Remove(link); }
            onChanged();
        }
        #endregion

        #region StartQuest
        public delegate void OnQuestCompleted();
        public event OnQuestCompleted onQuestCompleted;

        private List<Objective> incompleteObjectives;

        public void Activate(GameObject objectiveTracker) {
            foreach (var link in dependencies) {
                { Objectives[link.childID].AddPrerequisite(Objectives[link.parentID]); }
            }

            incompleteObjectives = new List<Objective>(Objectives.Values);
            foreach(var objective in Objectives.Values) {
                objective.Activate(objectiveTracker);

                objective.onCompleted += () => CompleteObjective(objective);
            }
        }

        private void CompleteObjective(Objective objective) {
            incompleteObjectives.Remove(objective);
            if (incompleteObjectives.Count == 0) { onQuestCompleted(); }
        }
        #endregion
    }

    [System.Serializable]
    public struct Link
    {
        public int parentID;
        public int childID;

        public Link(int parentID, int childID) {
            this.parentID = parentID;
            this.childID = childID;
        }
    }
}