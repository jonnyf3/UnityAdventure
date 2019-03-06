using RPG.Characters;

namespace RPG.States
{
    public class IdleState : State
    {
        public override void OnStateEnter()
        {
            base.OnStateEnter();

            var ai = character as AICharacter;
            if (ai) { ai.StopMoving(); }
        }
    }
}