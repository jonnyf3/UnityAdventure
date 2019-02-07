using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private Animator animator;

        [Header("Moving")]
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float groundCheckDistance = 1f;
        [SerializeField] float animSpeedMultiplier = 1f;
        [SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others

        [Header("Jumping")]
        [SerializeField] float jumpPower = 6f;
        [SerializeField][Range(1f, 4f)] float gravityMultiplier = 2f;

        private bool isGrounded = true;
        private float startGroundCheckDistance;
        private Vector3 groundNormal;

        void Awake() {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator = GetComponentInChildren<Animator>();
            Assert.IsNotNull(animator, "Characters require an Animator on their Body to move");

            startGroundCheckDistance = groundCheckDistance;
        }

        public void Move(Vector3 move, bool jump) {
            // Confirm whether the character is on the ground or not
            CheckGroundStatus();

            if (isGrounded) {
                // check whether conditions are right to allow a jump
                if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) {
                    Jump();
                }
            }
            else { HandleAirborneMovement(); }

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            move = NormalizeMoveVector(move);
            //maximum forward movement when facing the direction of the input
            var turnAmount = Mathf.Atan2(move.x, move.z);
            var forwardAmount = move.z;

            // Help the character turn faster (this is in addition to root rotation in the animation)
            //ApplyExtraTurnRotation(turnAmount, forwardAmount);

            // send input and other state parameters to the animator
            UpdateAnimator(forwardAmount, turnAmount);

            //Transfer any movement from the body object (with the animator on) to the character object
            transform.position = animator.transform.position;
            animator.transform.localPosition = Vector3.zero;
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


        private Vector3 NormalizeMoveVector(Vector3 input) {
            if (input.magnitude > 1f) { input.Normalize(); }

            var adjustedMovement = transform.InverseTransformDirection(input);
            adjustedMovement = Vector3.ProjectOnPlane(adjustedMovement, groundNormal);
            
            return adjustedMovement;
        }

        void ApplyExtraTurnRotation(float turn, float forward) {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forward);
            float rotation = turn * turnSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        void UpdateAnimator(float forward, float turn) {
            // update the animator parameters
            animator.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);
            animator.SetBool("OnGround", isGrounded);
            if (!isGrounded) {
                animator.SetFloat("Jump", rigidbody.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float runCycle = Mathf.Repeat(animationTime + runCycleLegOffset, 1);

            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forward;
            if (isGrounded) { animator.SetFloat("JumpLeg", jumpLeg); }
        }

        //public void OnAnimatorMove() {
        //    // we implement this function to override the default root motion.
        //    // this allows us to modify the positional speed before it's applied.
        //    if (isGrounded && Time.deltaTime > 0) {
        //        Vector3 v = (animator.deltaPosition * 1f) / Time.deltaTime;

        //        // we preserve the existing y part of the current velocity.
        //        v.y = rigidbody.velocity.y;
        //        rigidbody.velocity = v;
        //    }
        //}
    }
}