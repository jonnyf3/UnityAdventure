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

        public List<Objective> objectives = new List<Objective>();
        public void AddObjective(Objective objective) {
            objectives.Add(objective);
            onChanged();
        }
        public void Delete(Objective objective) {
            objectives.Remove(objective);
            onChanged();
        }

        public void Activate() {

        }
    }
}