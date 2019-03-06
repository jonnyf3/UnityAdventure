using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public class AttackingState : CombatState
    {
        private float attacksPerSecond => (character as Enemy).AttacksPerSecond;
        private float lastAttackTime;

        protected override void Start() {
            base.Start();
            
            lastAttackTime = Time.deltaTime;
            ai.StopMoving();
        }

        private void Update() {
            if (distanceToTarget > attackRadius) {
                character.SetState<ChasingState>();
            }

            ai.TurnTowardsTarget(target);
            //Attack only when looking (roughly) towards the target
            Vector3 unitVectorToTarget = (target.position - transform.position).normalized;
            float angleTowardsTarget = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
            if (angleTowardsTarget < 7f) {
                //Check if shot to target is clear
                if (IsShotBlocked()) {
                    character.SetState<StrafingState>();
                    return;
                }

                //Randomise attack frequency
                var attackVariance = Random.Range(0.6f, 1.4f);
                var attackPeriod = (1f / attacksPerSecond) * attackVariance;
                if (Time.time - lastAttackTime >= attackPeriod) {
                    combat.Attack();
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}