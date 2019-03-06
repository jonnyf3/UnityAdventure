using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public abstract class State : MonoBehaviour
    {
        protected Character character;

        public virtual void OnStateEnter() {
            character = GetComponent<Character>();
        }
        public virtual void OnStateExit() { }
    }
}