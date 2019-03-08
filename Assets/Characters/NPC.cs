using UnityEngine;
using RPG.States;
using RPG.UI;

namespace RPG.Characters
{
    public class NPC : AICharacter
    {
        [Header("NPC")]
        [SerializeField] new string name = "";
        [SerializeField] float activationRadius = 6f;

        private Player player;

        public bool isActive { get; set; }

        protected override void Start() {
            base.Start();

            player = FindObjectOfType<Player>();
            allyState = AllyState.Neutral;
            isActive = true;

            GetComponentInChildren<CharacterUI>().SetUIText(name);

            SetState<NPCActiveState>();
        }

        protected override void DetermineState() {
            var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= activationRadius) {
                SetState<NPCActiveState>();
            } else {
                if (!(currentState as IdleState)) { SetState<IdleState>(); }
            }
        }
    }
}