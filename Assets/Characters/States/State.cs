using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public abstract class State : MonoBehaviour
    {
        protected Character character;

        public virtual void OnStateEnter(StateArgs args) {
            SetArgs(args);
        }
        public virtual void OnStateExit() { }

        public virtual void SetArgs(StateArgs args) {
            this.character = args.character;
        }
    }

    public class StateArgs
    {
        public Character character;

        public StateArgs(Character character)
        {
            this.character = character;
        }
    }
}