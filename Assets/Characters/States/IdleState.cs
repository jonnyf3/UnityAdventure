using RPG.Characters;

namespace RPG.States
{
    public class IdleState : State
    {
        public override void OnStateEnter() {
            base.OnStateEnter();

            if ((character as AICharacter)?.PatrolPath) {
                character.SetState<PatrollingState>();
            }
        }

        private void Update() {
            if (character as Player) { return; }
            if (character as NPC) { return; } //TODO

            if ((character as Enemy).Target) {
                character.SetState<AttackingState>(); //TODO this is probably bad
            }
        }
    }
}