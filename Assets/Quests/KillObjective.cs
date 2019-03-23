using UnityEngine;
using RPG.Combat;

namespace RPG.Quests
{
    [System.Serializable]
    public class KillObjective : Objective
    {
        public Health Target;

        public KillObjective(Vector2 position) : base(position) { }
    }
}