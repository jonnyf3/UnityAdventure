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

        private const string ANIMATOR_ACTIVATE_TRIGGER = "onActivate";
        private const string ANIMATOR_DEACTIVATE_TRIGGER = "onDeactivate";
        private bool isActive = true;

        private Player player;

        protected override void Start() {
            base.Start();

            player = FindObjectOfType<Player>();
            allyState = AllyState.Neutral;

            GetComponentInChildren<CharacterUI>().SetUIText(name);
        }

        void Update() {
            Move();

            var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= activationRadius) {
                Activate();
                TurnTowardsTarget(player.transform);
            }
            else {
                if (patrolPath) {
                    SetState<PatrollingState>();
                } else {
                    Deactivate();
                }
            }
        }

        private void Activate() {
            //TODO NPC does not stop patrolling if it has a patrol path
            SetState<IdleState>();
            if (!isActive) {
                animator.SetTrigger(ANIMATOR_ACTIVATE_TRIGGER);
                isActive = true;
            }
        }
        private void Deactivate() {
            SetState<IdleState>();
            if (isActive) {
                animator.SetTrigger(ANIMATOR_DEACTIVATE_TRIGGER);
                isActive = false;
            }
        }
    }
}