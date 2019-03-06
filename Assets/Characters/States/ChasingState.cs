namespace RPG.States
{
    public class ChasingState : CombatState
    {
       protected override void Start() {
            base.Start();
        }

        private void Update() {
            ai.SetMoveTarget(target.position);

            if (distanceToTarget <= attackRadius) {
                character.SetState<AttackingState>();
            }
        }
    }
}