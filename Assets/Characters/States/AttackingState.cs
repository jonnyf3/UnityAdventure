using UnityEngine;

namespace RPG.States
{
    public class AttackingState : CombatState
    {
        private float minAttackAngle = 7f;

        protected override void Start() {
            base.Start();
            ai.StopMoving();
        }

        private void Update() {
            ai.SetLookTarget(Target);

            if (!Target) {
                character.SetState<IdleState>();
                return;
            }
            //Allow for small changes in position
            if (distanceToTarget > attackRadius * 1.1f) {
                character.SetState<ChasingState>();
                return;
            }

            //Attack only when looking (roughly) towards the target
            if (distanceToTarget >= 1f && Mathf.Abs(angleToTarget) >= minAttackAngle) { return; }

            //Check if shot to target is clear
            if (IsShotBlocked()) {
                character.SetState<StrafingState>();
                return;
            }

            combat.Attack();
        }
    }
}