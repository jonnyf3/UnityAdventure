using UnityEngine;
using RPG.Movement;

namespace RPG.States
{
    public class StrafingState : CombatState
    {
        private float direction;

        protected override void Start() {
            base.Start();
            character.GetComponent<CharacterMovement>().Focussed = true;

            direction = Mathf.Sign(Random.Range(-1f, 1f));
        }

        void Update() {
            if (!IsShotBlocked()) { character.SetState<AttackingState>(); }

            /* Constantly set a move target directly perpendicular to the character - combined with
            a rotation to look towards the target, this results in circular movement around the target */
            Vector3 unitVectorToTarget = (target.position - transform.position).normalized;
            transform.forward = Vector3.ProjectOnPlane(unitVectorToTarget, Vector3.up);

            //new position needs to be further away than stopping distance
            var newPos = 4 * (direction * transform.right);
            ai.SetMoveTarget(transform.position + newPos);
        }

        protected override void OnDestroy() {
            character.GetComponent<CharacterMovement>().Focussed = false;
            base.OnDestroy();
        }
    }
}