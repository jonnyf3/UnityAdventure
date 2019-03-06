using RPG.Characters;

namespace RPG.States
{
    public class IdleState : State
    {
        public override void Start() {
            base.Start();

            var ai = character as AICharacter;
            if (ai) { ai.StopMoving(); }
        }
    }
}