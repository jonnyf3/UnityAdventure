using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        private Animator animator;
        private new Rigidbody rigidbody;

        [Header("Moving")]
        [SerializeField][Range(0.1f, 1f)] float animatorForwardCap = 1f;  //Limit the maximum "forward" amount sent to the animator
        [SerializeField] float moveSpeedMultiplier = 1.25f;
        [SerializeField] float animSpeedMultiplier = 1f;
        [SerializeField] float extraTurnSpeed = 360f;
        [SerializeField] float groundCheckDistance = 1f;
        [SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others

        [Header("Jumping")]
        [SerializeField] float jumpPower = 6f;
        [SerializeField][Range(1f, 4f)] float gravityMultiplier = 2f;

        private bool isGrounded = true;
        private float startGroundCheckDistance;
        private Vector3 groundNormal;

        public float AnimatorForwardCap { set { animatorForwardCap = value; } }

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator, "No animator found on " + gameObject);

            startGroundCheckDistance = groundCheckDistance;
        }

        public void Move(Vector3 movementDirection, bool jump, bool focussed) {
            if (movementDirection.magnitude > 1f) { movementDirection = movementDirection.normalized; }
            
            // Confirm whether the character is on the ground or not
            CheckGroundStatus();

            if (isGrounded) {
                // check whether conditions are right to allow a jump
                if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) {
                    Jump();
                }
            }
            else { HandleAirborneMovement(); }

            // Determine movement amount and turn based on this angle
            float forward = Vector3.Dot(transform.forward, movementDirection.normalized);
            forward = Mathf.Clamp(forward, -animatorForwardCap, animatorForwardCap);

            float horizontal = 0;
            if (!focussed) {
                //turning
                var theta = Vector3.SignedAngle(transform.forward, movementDirection, Vector3.up) * Mathf.Deg2Rad;
                horizontal = Mathf.Sin(theta / 2);

                // Help the character turn faster (this is in addition to root movement from the animation)
                ApplyExtraTurnRotation(horizontal);
            } else {
                //strafing
                horizontal = Vector3.Dot(transform.right, movementDirection.normalized);
            }
            
            UpdateAnimator(forward, horizontal, focussed);
        }

        void CheckGroundStatus() {
            RaycastHit hitInfo;
            isGrounded = Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance);

            if (isGrounded) {
                groundNormal = hitInfo.normal;
                animator.applyRootMotion = true;
                animator.speed = animSpeedMultiplier;
            }
            else {
                groundNormal = Vector3.up;
                animator.applyRootMotion = false;
                animator.speed = 1;
            }
        }

        void Jump() {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpPower, rigidbody.velocity.z);
            isGrounded = false;
            animator.applyRootMotion = false;
            groundCheckDistance = 0.1f;
        }
        void HandleAirborneMovement() {
            // apply extra gravity
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigidbody.AddForce(extraGravityForce);

            groundCheckDistance = rigidbody.velocity.y < 0 ? startGroundCheckDistance : 0.01f;
        }

        void OnAnimatorMove() {
            // Apply extra movement speed
            if (isGrounded && Time.deltaTime > 0) {
                Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // preserve the existing y part of the current velocity.
                v.y = rigidbody.velocity.y;
                rigidbody.velocity = v;
            }
        }
        void ApplyExtraTurnRotation(float turn) {
            float rotation = turn * extraTurnSpeed * Time.deltaTime;
            animator.transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        void UpdateAnimator(float forward, float horizontal, bool focussed) {
            // Update the animator parameters
            animator.SetBool("Focussed", focussed);

            animator.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
            animator.SetBool("OnGround", isGrounded);
            if (!isGrounded) {
                animator.SetFloat("Jump", rigidbody.velocity.y);
            }

            SetJumpLeg(forward);
        }

        void SetJumpLeg(float forward) {
            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float runCycle = Mathf.Repeat(animationTime + runCycleLegOffset, 1);

            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forward;
            if (isGrounded) { animator.SetFloat("JumpLeg", jumpLeg); }
        }
    }
}