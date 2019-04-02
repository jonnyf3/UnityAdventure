using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public abstract class State : MonoBehaviour
    {
        protected Character character;

        protected virtual void Awake() {
            character = GetComponent<Character>();
        }
    }
}