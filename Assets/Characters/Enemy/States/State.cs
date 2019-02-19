using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public abstract class State : MonoBehaviour
    {
        protected AICharacter character;

        public virtual void OnStateEnter(StateArgs args) {
            this.character = args.character;
        }
    }

    public class StateArgs
    {
        public AICharacter character;

        public StateArgs(AICharacter character)
        {
            this.character = character;
        }
    }
}