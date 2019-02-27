using UnityEngine;
using RPG.States;
using RPG.UI;

namespace RPG.Characters
{
    public class NPCController : AICharacter
    {
        [Header("NPC")]
        [SerializeField] new string name = "";
        [SerializeField] float activationRadius = 6f;

        private const string ANIMATOR_ACTIVATE_TRIGGER = "onActivate";
        private const string ANIMATOR_DEACTIVATE_TRIGGER = "onDeactivate";
        private bool isActive = true;

        private PlayerController player;

        protected override void Start() {
            base.Start();

            player = FindObjectOfType<PlayerController>();
            allyState = AllyState.Neutral;

            GetComponentInChildren<CharacterUI>().SetUIText(name);
        }

        void Update() {
            Move();

            var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= activationRadius) {
                if (!isActive) {
                    animator.SetTrigger(ANIMATOR_ACTIVATE_TRIGGER);
                    isActive = true;
                }
                SetState<IdleState>(new StateArgs(this));

                TurnTowardsTarget(player.transform);
            }
            else {
                if (patrolPath) {
                    var patrolArgs = new PatrollingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
                    SetState<PatrollingState>(patrolArgs);
                } else {
                    if (isActive) {
                        animator.SetTrigger(ANIMATOR_DEACTIVATE_TRIGGER);
                        isActive = false;
                    }
                    SetState<IdleState>(new StateArgs(this));
                }
            }
        }

        public override void Die() {
            base.Die();
            health.Respawn();
        }
    }
}