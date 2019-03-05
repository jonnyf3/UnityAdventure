using RPG.Characters;

namespace RPG.States
{
    public class IdleState : State
    {
        public override void OnStateEnter() {
            base.OnStateEnter();

            if ((character as AICharacter)?.PatrolPath) {
                character.SetState<PatrollingState>();
                return;
            }

            if (character as Enemy) {
                (character as Enemy).onEnterAttackingState += Attack;
                (character as Enemy).onEnterChasingState   += Chase;
            }
        }
    }
}