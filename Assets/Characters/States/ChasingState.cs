namespace RPG.States
{
    public class ChasingState : CombatState
    {
        private void Update() {
            ai.SetMoveTarget(Target.position);

            if (distanceToTarget <= attackRadius) {
                character.SetState<AttackingState>();
            }
        }
    }
}