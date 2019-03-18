using UnityEngine;
using UnityEngine.AI;
using RPG.Movement;
using RPG.Characters;

namespace RPG.States
{
    public class FallingState : State
    {
        private Animator animator;
        private CharacterMovement movement;

        private const float FALL_TIME_LIMIT = 0.8f;
        private float fallingTime;

        protected override void Start() {
            base.Start();
            movement = GetComponent<CharacterMovement>();
            animator = GetComponent<Animator>();

            animator.applyRootMotion = false;
            animator.speed = 1;
            animator.SetBool("isGrounded", false);

            if (character as AICharacter) { GetComponent<NavMeshAgent>().enabled = false; }

            // apply extra gravity
            //Vector3 extraGravityForce = (Physics.gravity * 1.5f) - Physics.gravity;
            //GetComponent<Rigidbody>().AddForce(extraGravityForce);

            fallingTime = 0;
        }

        void Update() {
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, movement.GroundCheckDistance)) {
                if (character as Player) {
                    (character as Player).SetControlled();
                    return;
                } else {
                    character.SetState<IdleState>();
                    return;
                }
            }

            fallingTime += Time.deltaTime;
            if (fallingTime > FALL_TIME_LIMIT) { character.SetState<DeadState>(); }
        }

        private void OnDestroy() {
            if (character as AICharacter) { GetComponent<NavMeshAgent>().enabled = true; }

            if (animator) {
                animator.applyRootMotion = true;
                animator.speed = movement.MoveSpeed;
                animator.SetBool("isGrounded", true);
            }
        }
    }
}