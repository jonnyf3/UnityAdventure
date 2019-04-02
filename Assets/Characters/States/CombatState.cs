using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;

namespace RPG.States
{
    public class CombatState : State
    {
        protected CombatSystem combat;

        protected Transform Target => (character as Enemy).Target;

        private Vector3 vectorToTarget => Target.position - transform.position;
        protected float distanceToTarget => vectorToTarget.magnitude;
        protected float angleToTarget => Vector3.SignedAngle(vectorToTarget, transform.forward, Vector3.up);

        protected float attackRadius => combat.AttackRange;

        protected virtual void Start() {
            Assert.IsNotNull((character as Enemy), "Combat States should only be entered by an Enemy character");
            
            combat = GetComponent<CombatSystem>();
        }

        protected bool IsShotBlocked() {
            var hit = Physics.Raycast(transform.position + new Vector3(0, 1f, 0),
                                      vectorToTarget.normalized, out RaycastHit hitInfo,
                                      distanceToTarget, ~0, QueryTriggerInteraction.Ignore);
            if (!hit) { return false; }
            return hitInfo.collider.transform != Target;
        }
    }
}