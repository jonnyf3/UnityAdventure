namespace RPG.States
{
    public class InvestigatingState : CombatState
    {
        private float baseAnimatorForwardCap;

        protected override void Start() {
            base.Start();
            
            baseAnimatorForwardCap = movement.AnimatorForwardCap;
            movement.AnimatorForwardCap = 0.5f;
        }

        private void Update() {
            ai.SetMoveTarget(target.position);
        }

        private void OnDestroy() {
            movement.AnimatorForwardCap = baseAnimatorForwardCap;
        }
    }
}