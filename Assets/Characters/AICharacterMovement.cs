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
            if (Target == null) { return; }

            agent.SetDestination(Target.position);
            
            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (arrivedAtTarget) {
                Move(Vector3.zero, false);
            }
            else {
                Move(agent.desiredVelocity, false);
            }

            //Stop the AI main character body rotating - the superposition of base rotation + body rotation does weird things
            //TODO work out why this is happening?
            transform.rotation = Quaternion.identity;
        }
    }
}