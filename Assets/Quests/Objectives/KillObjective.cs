using UnityEngine;
using System.Collections.Generic;

namespace RPG.Quests
{
    [System.Serializable]
    public class KillObjective : Objective
    {
        public List<string> Targets; //todo store GUIDs rather than names

        public KillObjective(Vector2 position) : base(position) { }
    }
}