﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Movement;
using RPG.Combat;

namespace RPG.States
{
    public class AttackingState : CombatState
    {
        private WeaponSystem combat;
        
        private float attacksPerSecond;
        private float lastAttackTime;

        public override void OnStateEnter() {
            base.OnStateEnter();

            Assert.IsNotNull((character as Enemy), "AttackingState should only be entered by Enemy characters");
            attacksPerSecond = (character as Enemy).AttacksPerSecond;

            combat = GetComponent<WeaponSystem>();
            lastAttackTime = Time.deltaTime;
            StartCoroutine(Attack());
        }

        private void Update() {
            if (target && distanceToTarget > attackRadius) {
                character.SetState<ChasingState>();
            }
        }

        private IEnumerator Attack() {
            while (true) {
                TurnTowards(target);
                //Attack only when looking (roughly) towards the target
                Vector3 unitVectorToTarget = (target.position - transform.position).normalized;
                float angleTowardsTarget = Mathf.Abs(Vector3.SignedAngle(unitVectorToTarget, transform.forward, Vector3.up));
                if (angleTowardsTarget < 7f) {
                    //Check if shot to target is clear
                    if (IsShotBlocked()) {
                        yield return StartCoroutine(MoveToClearLineOfSight());
                    }

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

        private bool IsShotBlocked() {
            int mask = ~0;
            Vector3 vectorToTarget = target.position - transform.position;
            var hit = Physics.Raycast(transform.position + new Vector3(0, 1f, 0),
                                      vectorToTarget.normalized, out RaycastHit hitInfo,
                                      vectorToTarget.magnitude, mask, QueryTriggerInteraction.Ignore);
            if (!hit) { return false; }
            return hitInfo.collider.transform != target;
        }

        private IEnumerator MoveToClearLineOfSight() {
            var movement = character.GetComponent<CharacterMovement>();
            movement.Focussed = true;
            var direction = Mathf.Sign(Random.Range(-1f, 1f));

            while (IsShotBlocked()) {
                MoveAroundTarget(direction);
                yield return new WaitForEndOfFrame();
            }
            
            movement.Focussed = false;
        }
        private void MoveAroundTarget(float direction) {
            /* Constantly set a move target directly perpendicular to the character - combined with
            a rotation to look towards the target, this results in circular movement around the target */
            Vector3 unitVectorToTarget = (target.position - transform.position).normalized;
            transform.forward = Vector3.ProjectOnPlane(unitVectorToTarget, Vector3.up);

            //new position needs to be further away than stopping distance
            var newPos = 4 * (direction * transform.right);
            MoveTowards(transform.position + newPos);
        }

        private void OnDestroy() {
            StopAllCoroutines();
            character.GetComponent<CharacterMovement>().Focussed = false;
        }
    }
}