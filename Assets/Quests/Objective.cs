using UnityEngine;

namespace RPG.Quests
{
    [System.Serializable]
    public abstract class Objective
    {
        public Vector2 position { get; }

        public Objective(Vector2 position) {
            this.position = position;
        }
    }
}