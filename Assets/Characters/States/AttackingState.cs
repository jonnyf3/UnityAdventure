using UnityEngine;

namespace RPG.States
{
    public class AttackingState : CombatState
    {
        protected override void Start() {
            base.Start();
            character.Move(transform.position);
        }

        private void Update() {
            character.TurnTowards(Target);

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
            if (Vector3.Dot(vectorToTarget.normalized, transform.forward) <= 0.75f) { return; }

            //Check if shot to target is clear
            if (IsShotBlocked()) {
                character.SetState<StrafingState>();
                return;
            }

            combat.Attack(Target);
        }
    }
}