using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = ("RPG/Quest"))]
    public class Quest : ScriptableObject
    {
        public delegate void OnChanged();
        public event OnChanged onChanged;

        public delegate void OnQuestCompleted();
        public event OnQuestCompleted onQuestCompleted;

        public string questName;
        public int experiencePoints;

        //Serialization (to save Quest SO as an asset) does not support inheritance
        //Need a separate list of each base objective type in order to properly store their data
        //Combine these lists together as a property at run-time, rather than saving it
        private List<KillObjective> killObjectives = new List<KillObjective>();
        private List<TravelObjective> travelObjectives = new List<TravelObjective>();
        public List<Objective> Objectives {
            get {
                var objectives = new List<Objective>();
                foreach (var k in killObjectives) { objectives.Add(k); }
                foreach (var t in travelObjectives) { objectives.Add(t); }
                return objectives;
            }
        }
        
        public void AddObjective(Objective objective) {
            Undo.RecordObject(this, "Add objective");

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

            if (objective as KillObjective != null) {
                killObjectives.Remove(objective as KillObjective);
            }
            if (objective as TravelObjective != null) {
                travelObjectives.Remove(objective as TravelObjective);
            }
            onChanged();
        }

        private List<Objective> incompleteObjectives;
        public void Activate(GameObject objectiveTracker) {
            incompleteObjectives = new List<Objective>(Objectives);
            foreach(var objective in Objectives) {
                objective.onObjectiveCompleted += () => CompleteObjective(objective);
                //TODO add only if objective is active (i.e. has no pre-requisites)
                if ((objective as KillObjective) != null) {
                    var objectiveComponent = objectiveTracker.AddComponent<KillObjectiveBehaviour>();
                    objectiveComponent.Setup(objective);
                }
                //TODO add behaviours for other cases
            }
        }

        private void CompleteObjective(Objective objective) {
            incompleteObjectives.Remove(objective);
            if (incompleteObjectives.Count == 0) { onQuestCompleted(); }
        }
    }
}