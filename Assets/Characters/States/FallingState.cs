using UnityEngine;
using UnityEngine.AI;
using RPG.Characters;
using RPG.Combat;

namespace RPG.States
{
    public class FallingState : State
    {
        private Animator animator;
        private float animatorStartSpeed;

        private const float MAX_FALL_DISTANCE = 8f;
        private const float MAX_FALL_TIME = 8f;
        private const float FALL_SAFE_DISTANCE = 2f;
        private const float FALL_DAMAGE_PER_METRE = 40f;

        private const string ANIMATOR_GROUND_BOOL = "isGrounded";

        private float startHeight;
        private float fallStartTime;

        private void Start() {
            animator = GetComponent<Animator>();
            animatorStartSpeed = animator.speed;

            animator.applyRootMotion = false;
            animator.speed = 1;
            animator.SetBool(ANIMATOR_GROUND_BOOL, false);

            if (character as AICharacter) { GetComponent<NavMeshAgent>().enabled = false; }

            // apply extra gravity
            //Vector3 extraGravityForce = (Physics.gravity * 1.5f) - Physics.gravity;
            //GetComponent<Rigidbody>().AddForce(extraGravityForce);

            startHeight = transform.position.y;
            fallStartTime = Time.time;
        }

        private void Update() {
            if (character.IsOnGround) {
                character.SetDefaultState();
                return;
            }
            
            //catch for falling somewhere with no lower bound (or getting stuck in falling state)
            if (startHeight - transform.position.y > MAX_FALL_DISTANCE
                || Time.time - fallStartTime > MAX_FALL_TIME) {
                character.SetState<DeadState>();
            }
        }

        private void OnDestroy() {
            if (character as AICharacter) { GetComponent<NavMeshAgent>().enabled = true; }

            if (animator) {
                animator.applyRootMotion = true;
                animator.speed = animatorStartSpeed;
                animator.SetBool(ANIMATOR_GROUND_BOOL, true);
            }

            var fallHeight = startHeight - transform.position.y;
            if (fallHeight > FALL_SAFE_DISTANCE) {
                var fallDamage = (fallHeight - FALL_SAFE_DISTANCE) * FALL_DAMAGE_PER_METRE;
                character.GetComponent<Health>().TakeDamage(fallDamage, null);
            }
        }
    }
}