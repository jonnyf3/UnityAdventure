using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = ("RPG/Quest"))]
    public class Quest : ScriptableObject
    {
        public delegate void OnChanged();
        public event OnChanged onChanged;

        public string questName;
        public int experiencePoints;

        //Serialization (to save Quest SO as an asset) does not support inheritance
        //Need a separate list of each base objective type in order to properly save them
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
        
        public void AddObjective(KillObjective objective) {
            killObjectives.Add(objective);
            onChanged();
        }
        public void AddObjective(TravelObjective objective) {
            travelObjectives.Add(objective);
            onChanged();
        }
        public void Delete(Objective objective) {
            if (objective as KillObjective != null)   { killObjectives.Remove(objective as KillObjective); }
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