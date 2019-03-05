using UnityEngine.AI;

namespace RPG.States
{
    public class ChasingState : CombatState
    {
        public override void OnStateEnter() {
            base.OnStateEnter();
            GetComponent<NavMeshAgent>().isStopped = false;
        }

        private void Update() {
            MoveTowards(target.position);

            if (target && distanceToTarget < attackRadius) {
                character.SetState<AttackingState>();
            }
            if (distanceToTarget > chaseRadius) {
                character.SetState<IdleState>();
            }
        }

        private void OnDestroy() {
            GetComponent<NavMeshAgent>().isStopped = true;
            StopAllCoroutines();
        }
    }
}