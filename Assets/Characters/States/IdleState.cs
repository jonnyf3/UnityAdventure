using RPG.Characters;
using RPG.States;

public class IdleState : State
{
    public override void OnStateEnter(StateArgs args) {
        base.OnStateEnter(args);

        var ai = character as AICharacter;
        if (ai) { ai.StopMoving(); }
    }
}
