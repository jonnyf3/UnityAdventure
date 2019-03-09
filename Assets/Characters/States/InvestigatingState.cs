namespace RPG.States
{
    public class InvestigatingState : CombatState
    {
        protected override void Start() {
            base.Start();

            movement.SetAnimatorForwardCap(0.5f);
        }

        private void Update() {
            ai.SetMoveTarget(Target.position);
        }

        private void OnDestroy() {
            movement.ResetAnimatorForwardCap();
        }
    }
}