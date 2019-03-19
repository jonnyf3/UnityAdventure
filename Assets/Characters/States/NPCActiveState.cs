using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.States
{
    public class NPCActiveState : State
    {
        private const string ANIMATOR_ACTIVATE_TRIGGER = "onActivate";
        private const string ANIMATOR_DEACTIVATE_TRIGGER = "onDeactivate";

        private Animator animator;
        private Player player;

        protected override void Start() {
            base.Start();
            Assert.IsNotNull(character as NPC, "NPCActiveState should only be entered by an NPC character");
            
            animator = GetComponent<Animator>();
            player = FindObjectOfType<Player>();

            if (!(character as AICharacter).PatrolPath && !(character as NPC).isActive) {
                animator.SetTrigger(ANIMATOR_ACTIVATE_TRIGGER);
                (character as NPC).isActive = true;
            }
            character.Move(transform.position);
        }

        private void Update() {
            character.TurnTowards(player.transform);
        }

        void OnDestroy() {
            if (!(character as AICharacter).PatrolPath && (character as NPC).isActive) {
                animator.SetTrigger(ANIMATOR_DEACTIVATE_TRIGGER);
                (character as NPC).isActive = false;
            }
        }
    }
}