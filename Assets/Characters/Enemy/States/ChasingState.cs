using System.Collections;
using RPG.Characters;
using UnityEngine;

namespace RPG.States
{
    public class ChasingState : State
    {
        private Transform target;
       
        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            var chaseArgs = args as ChasingStateArgs;
            this.target = chaseArgs.target;

            StartCoroutine(Chase());
        }

        private IEnumerator Chase() {
            while (true) {
                character.SetMoveTarget(target.position);
                yield return new WaitForEndOfFrame();
            }
        }

        public void OnDestroy() {
            StopAllCoroutines();
            character.StopMoving();
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