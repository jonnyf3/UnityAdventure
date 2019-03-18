using UnityEngine;
using UnityEngine.AI;
using RPG.Characters;
using RPG.Movement;
using RPG.Combat;

namespace RPG.States
{
    public class FallingState : State
    {
        private Animator animator;
        private CharacterMovement movement;

        private const float MAX_FALL_DISTANCE = 5f;
        private const float FALL_SAFE_DISTANCE = 0.5f;
        private const float FALL_DAMAGE_PER_METRE = 40f;

        private float startHeight;

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

            startHeight = transform.position.y;
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
            
            //catch for falling somewhere with no lower bound
            if (startHeight - transform.position.y > MAX_FALL_DISTANCE) { character.SetState<DeadState>(); }
        }

        private void OnDestroy() {
            if (character as AICharacter) { GetComponent<NavMeshAgent>().enabled = true; }

            if (animator) {
                animator.applyRootMotion = true;
                animator.speed = movement.MoveSpeed;
                animator.SetBool("isGrounded", true);
            }

            var fallHeight = startHeight - transform.position.y;
            if (fallHeight > FALL_SAFE_DISTANCE) {
                var fallDamage = (fallHeight - FALL_SAFE_DISTANCE) * FALL_DAMAGE_PER_METRE;
                character.GetComponent<Health>().TakeDamage(fallDamage, null);
            }
        }
    }
}