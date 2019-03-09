﻿using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public class AttackingState : CombatState
    {
        private float attacksPerSecond => (character as Enemy).AttacksPerSecond;
        private float attackRandomnessFactor;
        private float lastAttackTime;

        private float minAttackAngle = 7f;

        protected override void Start() {
            base.Start();
            ai.StopMoving();

            lastAttackTime = 0;
            attackRandomnessFactor = Random.Range(0.6f, 1.4f);
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

            //Do attack if enough time has passed
            var attackPeriod = (1f / attacksPerSecond) * attackRandomnessFactor;
            if (Time.time - lastAttackTime >= attackPeriod) {
                combat.Attack();
                lastAttackTime = Time.time;
                attackRandomnessFactor = Random.Range(0.6f, 1.4f);
            }
        }
    }
}