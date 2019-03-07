namespace RPG.States
{
    public class ChasingState : CombatState
    {
        private void Update() {
            ai.SetMoveTarget(target.position);

            if (distanceToTarget <= attackRadius) {
                character.SetState<AttackingState>();
            }
        }
    }
}