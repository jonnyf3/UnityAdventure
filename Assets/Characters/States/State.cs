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

        protected void Attack() {
            character.SetState<AttackingState>();
        }
        protected void Chase() {
            character.SetState<ChasingState>();
        }
        protected void Idle() {
            character.SetState<IdleState>();
        }
    }
}