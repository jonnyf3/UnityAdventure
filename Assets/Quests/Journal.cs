using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] Quest quest;
        private GameObject activeObjectives;

        private void Start() {
            activeObjectives = new GameObject("Active Objectives");
            foreach (var o in quest.Objectives) {
                if (o as KillObjective != null) { print("KillObjective"); }
                else if (o as TravelObjective != null) { print("TravelObjective"); }
                else print("It's a base class");
            } 
            quest.Activate(activeObjectives);
        }
    }
}