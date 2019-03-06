using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.States
{
    public class ChasingState : State
    {
        private AICharacter ai;
        private Transform target => (character as Enemy).Target;
       
        public override void OnStateEnter() {
            base.OnStateEnter();
            ai = character as AICharacter;
            Assert.IsNotNull((character as Enemy), "ChasingState should only be entered by an Enemy character");

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
}