using System.Collections;
using UnityEngine;
using RPG.Characters;

namespace RPG.States
{
    public class ChasingState : State
    {
        private AICharacter ai;
        private Transform target;
       
        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            ai = character as AICharacter;

            var chaseArgs = args as ChasingStateArgs;
            this.target = chaseArgs.target;

            StartCoroutine(Chase());
        }

        private IEnumerator Chase() {
            while (true) {
                ai.SetMoveTarget(target.position);
                yield return new WaitForEndOfFrame();
            }
        }

        public override void OnStateExit() {
            StopAllCoroutines();
            ai.StopMoving();
        }
    }

    public class ChasingStateArgs : StateArgs
    {
        public Transform target;

        public ChasingStateArgs(AICharacter character, Transform target) : base(character) {
            this.target = target;
        }
    }
}