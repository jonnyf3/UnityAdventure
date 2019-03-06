using RPG.Characters;

namespace RPG.States
{
    public class IdleState : State
    {
        protected override void Start() {
            base.Start();

            if (character as AICharacter) {
                var ai = character as AICharacter;
                if (ai.PatrolPath) {
                    character.SetState<PatrollingState>();
                } else {
                    ai.StopMoving();
                }
            }
        }
    }
}