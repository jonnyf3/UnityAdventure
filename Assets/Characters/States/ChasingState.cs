using UnityEngine.AI;
using RPG.Characters;

namespace RPG.States
{
    public class ChasingState : CombatState
    {
        public override void OnStateEnter() {
            base.OnStateEnter();
            GetComponent<NavMeshAgent>().isStopped = false;

            (character as Enemy).onEnterAttackingState += Attack;
            character.onEnterIdleState += Idle;
        }

        private void Update() {
            MoveTowards(Target.position);
        }

        private void OnDestroy() {
            GetComponent<NavMeshAgent>().isStopped = true;
            StopAllCoroutines();
        }
    }
}