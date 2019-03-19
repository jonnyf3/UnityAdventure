namespace RPG.States
{
    public class InvestigatingState : CombatState
    {
        private void Update() {
            character.Move(Target.position, 0.5f);
        }
    }
}