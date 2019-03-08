using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;

namespace RPG.States
{
    public class CombatState : State
    {
        protected AICharacter ai;
        protected WeaponSystem combat;

        protected Transform target => (character as Enemy).Target;

        protected float distanceToTarget => Vector3.Distance(transform.position, target.position);
        protected float attackRadius => combat.CurrentWeapon.AttackRange;

        protected override void Start() {
            base.Start();

            ai = character as AICharacter;
            Assert.IsNotNull((character as Enemy), "Combat States should only be entered by an Enemy character");

            combat = GetComponent<WeaponSystem>();
        }

        protected bool IsShotBlocked() {
            int mask = ~0;
            Vector3 vectorToTarget = target.position - transform.position;
            var hit = Physics.Raycast(transform.position + new Vector3(0, 1f, 0),
                                      vectorToTarget.normalized, out RaycastHit hitInfo,
                                      vectorToTarget.magnitude, mask, QueryTriggerInteraction.Ignore);
            if (!hit) { return false; }
            return hitInfo.collider.transform != target;
        }
    }
}