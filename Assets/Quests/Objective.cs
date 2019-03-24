using UnityEngine;

namespace RPG.Quests
{
    public class Objective
    {
        public Vector2 position { get; set; }

        public Objective(Vector2 position) {
            this.position = position;
        }
    }
}