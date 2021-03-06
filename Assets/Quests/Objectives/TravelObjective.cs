﻿using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public class TravelObjective : Objective
    {
        public string Destination = "";
        public float RequiredProximity;

        public TravelObjective(Vector2 position) : base(position) { }

        protected override ObjectiveBehaviour AddBehaviour(GameObject objectiveTracker) {
            return objectiveTracker.AddComponent<TravelObjectiveBehaviour>();
        }
    }
}