namespace RPG.States
{
    public class ChasingState : CombatState
    {
        private void Update() {
            character.Move(Target.position);

            if (distanceToTarget <= attackRadius) {
                character.SetState<AttackingState>();
            }
        }
    }
}