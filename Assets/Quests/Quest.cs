using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = ("RPG/Quest"))]
    public class Quest : ScriptableObject
    {
        public delegate void OnChanged();
        public event OnChanged onChanged;

        public int experiencePoints;

        public List<Objective> Objectives {
            get {
                var objectives = new List<Objective>();
                foreach (var k in killObjectives) { objectives.Add(k); }
                foreach (var t in travelObjectives) { objectives.Add(t); }
                return objectives;
            }
        }
        
        private List<KillObjective> killObjectives = new List<KillObjective>();
        public void AddObjective(KillObjective objective) {
            killObjectives.Add(objective);
            onChanged();
        }
        private List<TravelObjective> travelObjectives = new List<TravelObjective>();
        public void AddObjective(TravelObjective objective) {
            travelObjectives.Add(objective);
            onChanged();
        }
        public void Delete(Objective objective) {
            if (objective as KillObjective != null) { killObjectives.Remove(objective as KillObjective); }
            if (objective as TravelObjective != null) { travelObjectives.Remove(objective as TravelObjective); }
            onChanged();
        }

        public void Activate(GameObject objectiveTracker) {
            foreach(var objective in Objectives) {
                if ((objective as KillObjective) != null) {
                    var objectiveComponent = objectiveTracker.AddComponent<KillObjectiveBehaviour>();
                    objectiveComponent.Setup(objective);
                }
                //TODO add other cases
            }
        }
    }
}