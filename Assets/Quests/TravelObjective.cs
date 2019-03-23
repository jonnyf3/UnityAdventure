using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class TravelObjective : Objective
    {
        public string Destination = "This is a Travel Objective";

        public TravelObjective(Vector2 position) : base(position) { }
    }
}