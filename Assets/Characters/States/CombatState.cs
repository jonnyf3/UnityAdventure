using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.States
{
    public class CombatState : AIState
    {
        /* Combat states are those which require a target, such as attacking or chasing,
         and so should only be accessed by Enemy Characters */
        
        protected Transform Target => (character as Enemy).Target;
        
        public override void OnStateEnter() {
            base.OnStateEnter();
            Assert.IsNotNull((character as Enemy), "This State should only be entered by Enemy characters");
        }
    }
}