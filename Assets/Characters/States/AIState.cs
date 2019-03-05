using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Movement;

namespace RPG.States
{
    public class AIState : State
    {
        private NavMeshAgent agent;
        private CharacterMovement movement;

        private float turnSpeed => (character as AICharacter).TurnSpeed;

        public override void OnStateEnter() {
            base.OnStateEnter();
            Assert.IsNotNull((character as AICharacter), "This State should only be entered by AI characters");

            agent = GetComponent<NavMeshAgent>();
            movement = GetComponent<CharacterMovement>();
        }

        protected void MoveTowards(Vector3 position) {
            agent.destination = position;

            //Process any required movement via the CharacterMovement component
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            Vector3 moveVector = arrivedAtTarget ? Vector3.zero : agent.desiredVelocity.normalized;

            movement.Move(moveVector);
        }

        protected void TurnTowards(Transform target) {
            Vector3 vectorToTarget = target.position - transform.position;
            Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToTarget, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotatedForward);
            transform.rotation.SetLookRotation(target.position - transform.position);
        }
    }
}