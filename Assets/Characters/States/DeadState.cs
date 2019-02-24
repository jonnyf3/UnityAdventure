using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public class DeadState : State
    {
        private const string ANIMATOR_DEATH_PARAM = "onDeath";

        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            var ai = character as AICharacter;
            if (ai) { ai.StopMoving(); }

            character.GetComponent<Animator>().SetTrigger(ANIMATOR_DEATH_PARAM);
            //Destroy(gameObject, 3f);  //this shouldn't apply to player or to npc
        }
    }
}