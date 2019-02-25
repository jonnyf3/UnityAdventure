using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.States;

namespace RPG.Characters
{
    public class NPCController : AICharacter
    {
        [Header("NPC")]
        [SerializeField] new string name = "";
        private Text displayText;

        [SerializeField] float activationRadius = 6f;
        private PlayerController player;

        protected override void Start() {
            base.Start();

            var displayText = ui.GetComponentInChildren<Text>();
            Assert.IsNotNull(displayText);
            displayText.text = name;

            player = FindObjectOfType<PlayerController>();

            allyState = AllyState.Neutral;
        }

        void Update() {
            Move();

            var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= activationRadius) {
                animator.SetBool(ANIMATOR_ACTIVATE_PARAM, true);
                SetState<IdleState>(new StateArgs(this));

                TurnTowardsTarget(player.transform);
            }
            else {
                if (patrolPath) {
                    var patrolArgs = new PatrollingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
                    SetState<PatrollingState>(patrolArgs);
                } else {
                    animator.SetBool(ANIMATOR_ACTIVATE_PARAM, false);
                    SetState<IdleState>(new StateArgs(this));
                }
            }

            agent.transform.localPosition = Vector3.zero;
            return;
        }

        public override void Die() {
            base.Die();
            //Immediately respawn
            health.RestoreHealth(1f);
        }
    }
}