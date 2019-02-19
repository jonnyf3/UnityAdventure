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
        [SerializeField] float activationRadius = 6f;
        private Text displayText;

        private PlayerController player;
        private bool playerInRange = false;
        
        protected override void Start() {
            base.Start();

            allyState = AllyState.Neutral;

            var displayText = ui.GetComponentInChildren<Text>();
            Assert.IsNotNull(displayText);
            displayText.text = name;

            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = activationRadius;

            player = FindObjectOfType<PlayerController>();
        }

        protected override void Update() {
            //Don't do movement from base update
            //TODO NPC animator doesn't contain required parameters
            if (playerInRange) {
                var idleArgs = new IdlingStateArgs(this, patrolPath, patrolWaypointDelay, patrolWaypointTolerance);
                SetState<IdlingState>(idleArgs);

                TurnTowardsTarget(player.transform);
            }
            else {
                //TODO should this be another state?
                StopMoving();
            }

            agent.transform.localPosition = Vector3.zero;
            return;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.GetComponent<PlayerController>()) {
                playerInRange = true;
                animator.SetBool("PlayerInRange", true);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.GetComponent<PlayerController>()) {
                playerInRange = false;
                animator.SetBool("PlayerInRange", false);
            }
        }

        public override void Die() {
            base.Die();
            health.RestoreHealth(1f);
        }
    }
}