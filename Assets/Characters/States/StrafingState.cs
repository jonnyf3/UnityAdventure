using UnityEngine;

namespace RPG.States
{
    public class StrafingState : CombatState
    {
        private float direction;

        protected override void Start() {
            base.Start();

            character.Focus(true);
            direction = Mathf.Sign(Random.Range(-1f, 1f));
        }

        private void Update() {
            if (!IsShotBlocked()) { character.SetState<AttackingState>(); }

            /* Constantly set a move target directly perpendicular to the character - combined with
            a rotation to look towards the target, this results in circular movement around the target */
            Vector3 unitVectorToTarget = (Target.position - transform.position).normalized;
            transform.forward = Vector3.ProjectOnPlane(unitVectorToTarget, Vector3.up);

            //new position needs to be further away than stopping distance
            var newPos = 4 * (direction * transform.right);
            character.Move(transform.position + newPos);
        }

        private void OnDestroy() {
            character.Focus(false);
        }
    }
}