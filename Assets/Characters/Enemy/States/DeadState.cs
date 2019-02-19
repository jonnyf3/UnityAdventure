namespace RPG.States
{
    public class DeadState : State
    {
        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            character.StopMoving();
        }
    }
}
