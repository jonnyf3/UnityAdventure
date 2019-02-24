using System.Collections;
using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public class AttackingState : State
    {
        private AICharacter ai;

        private Transform target;
        private WeaponSystem combat;
        private float attacksPerSecond;

        private float lastAttackTime;

        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            ai = character as AICharacter;

            var attackArgs = args as AttackingStateArgs;
            this.target = attackArgs.target;
            this.combat = attackArgs.weaponSystem;
            this.attacksPerSecond = attackArgs.attacksPerSecond;

            lastAttackTime = Time.deltaTime;

            ai.StopMoving();
            StartCoroutine(Attack());
        }

        private IEnumerator Attack() {
            while (true) {
                ai.TurnTowardsTarget(target);
                //Attack only when looking (roughly) towards the target
                Vector3 unitVectorToTarget = (target.position - transform.position).normalized;
                float angleTowardsTarget = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
                if (angleTowardsTarget < 7f) {
                    //Check if shot to target is clear
                    yield return StartCoroutine(MoveToClearLineOfSight());

                    //Randomise attack frequency
                    var attackVariance = Random.Range(0.6f, 1.4f);
                    var attackPeriod = (1f / attacksPerSecond) * attackVariance;
                    if (Time.time - lastAttackTime >= attackPeriod) {
                        combat.Attack();
                        lastAttackTime = Time.time;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator MoveToClearLineOfSight() {
            character.Focus(true);
            var direction = Mathf.Sign(Random.Range(-1f, 1f));

            while (IsShotBlocked()) {
                MoveAroundTarget(direction);
                yield return new WaitForEndOfFrame();
            }

            character.Focus(false);
            ai.StopMoving();
        }

        private bool IsShotBlocked() {
            int mask = ~0;
            Vector3 vectorToTarget = target.position - transform.position;
            var hit = Physics.Raycast(transform.position + new Vector3(0, 1f, 0),
                                      vectorToTarget.normalized, out RaycastHit hitInfo,
                                      vectorToTarget.magnitude, mask, QueryTriggerInteraction.Ignore);
            if (!hit) { return false; }
            return hitInfo.collider.transform != target;
        }
        private void MoveAroundTarget(float direction) {
            /* Constantly set a move target directly perpendicular to the character - combined with
            a rotation to look towards the target, this results in circular movement around the target */
            Vector3 unitVectorToTarget = (target.position - transform.position).normalized;
            transform.forward = Vector3.ProjectOnPlane(unitVectorToTarget, Vector3.up);

            //new position needs to be further away than stopping distance
            var newPos = 4 * (direction * transform.right);
            ai.SetMoveTarget(transform.position + newPos);
        }

        public override void OnStateExit() {
            StopAllCoroutines();
            character.Focus(false);
            ai.StopMoving();
        }
    }

    public class AttackingStateArgs : StateArgs
    {
        public Transform target;
        public WeaponSystem weaponSystem;
        public float attacksPerSecond;

        public AttackingStateArgs(AICharacter character, Transform target, WeaponSystem weaponSystem, float attacksPerSecond) : base(character)
        {
            this.target = target;
            this.weaponSystem = weaponSystem;
            this.attacksPerSecond = attacksPerSecond;
        }
    }
}