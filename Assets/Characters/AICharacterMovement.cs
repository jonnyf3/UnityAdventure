using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class AICharacterMovement : CharacterMovement
    {
        public Transform Target { get; set; }

        private NavMeshAgent agent;

        private void Start() {
            agent = GetComponentInChildren<NavMeshAgent>();
            Assert.IsNotNull(agent, "AI Characters must have NavMesh Agents on their Body");
            agent.updateRotation = false;
            agent.updatePosition = true;
        }

        void Update() {
            if (Target != null) { agent.SetDestination(Target.position); }

            bool arrivedAtTarget = (agent.remainingDistance > agent.stoppingDistance);
            if (arrivedAtTarget) {
                Move(agent.desiredVelocity, false);
            } else {
                Move(Vector3.zero, false);
            }
        }
    }
}