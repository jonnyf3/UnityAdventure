using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.States
{
    public class NPCActiveState : State
    {
        private AICharacter ai;
        private Animator animator;
        private Player player;

        private const string ANIMATOR_ACTIVATE_TRIGGER = "onActivate";
        private const string ANIMATOR_DEACTIVATE_TRIGGER = "onDeactivate";

        protected override void Start() {
            base.Start();
            Assert.IsNotNull(character as NPC, "NPCActiveState should only be entered by an NPC character");

            ai = character as AICharacter;
            animator = GetComponent<Animator>();

            player = FindObjectOfType<Player>();

            ai.StopMoving();
            if (!ai.PatrolPath && !(character as NPC).isActive) {
                animator.SetTrigger(ANIMATOR_ACTIVATE_TRIGGER);
                (character as NPC).isActive = true;
            }
        }
        
        void Update() {
            ai.TurnTowardsTarget(player.transform);
        }

        void OnDestroy() {
            if (!ai.PatrolPath && (character as NPC).isActive) {
                animator.SetTrigger(ANIMATOR_DEACTIVATE_TRIGGER);
                (character as NPC).isActive = false;
            }
        }
    }
}