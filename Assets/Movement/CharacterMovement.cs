using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        private Animator animator;
        private new Rigidbody rigidbody;
        
        [Header("Moving")]
        [SerializeField] float moveSpeedMultiplier = 1.25f;
        [SerializeField] float extraTurnSpeed = 360f;
        [SerializeField][Range(0.1f, 1f)] float animatorForwardCap = 1f;  //Limit the maximum "forward" amount sent to the animator

        [Header("Ground Check")]
        [SerializeField] float groundCheckDistance = 1f;
        [SerializeField][Range(1f, 4f)] float gravityMultiplier = 2f;

        public bool Focussed { get; set; }

        private float baseAnimatorForwardCap;
        public void SetAnimatorForwardCap(float value) { animatorForwardCap = value; }
        public void ResetAnimatorForwardCap() { animatorForwardCap = baseAnimatorForwardCap; }

        void Start() {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator, "No animator found on " + gameObject);
            animator.applyRootMotion = true;
            animator.speed = moveSpeedMultiplier;

            baseAnimatorForwardCap = animatorForwardCap;
        }

        public void Move(Vector3 direction) {
            var localMovement = transform.InverseTransformDirection(direction);
            
            float forward = Mathf.Clamp(localMovement.z, -animatorForwardCap, animatorForwardCap);
            //Ensure large turning when desired direction is directly backwards
            float horizontal = localMovement.x;
            if (forward < 0 && !Focussed) { horizontal += Mathf.Abs(forward); }
            // Check for falling
            bool isGrounded = CheckGroundStatus();

            ApplyExtraTurnRotation(horizontal);
            UpdateAnimator(forward, horizontal, isGrounded, Focussed);
        }

        public void TurnTowards(Transform target) {
            Vector3 vectorToTarget = target.position - transform.position;
            float requiredTurn = Vector3.SignedAngle(transform.forward, vectorToTarget, Vector3.up);
            if (Mathf.Abs(requiredTurn) >= 5f) {
                UpdateAnimator(0f, Mathf.Sign(requiredTurn), true, false);
            } else {
                UpdateAnimator(0f, 0f, true, false);
            }
        }

        private bool CheckGroundStatus() {
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, groundCheckDistance)) {
                animator.applyRootMotion = true;
                animator.speed = moveSpeedMultiplier;
                return true;
            }
            else {
                animator.applyRootMotion = false;
                animator.speed = 1;

                // apply extra gravity
                Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
                rigidbody.AddForce(extraGravityForce);

                return false;
            }
        }

        void ApplyExtraTurnRotation(float turn) {
            float rotation = turn * extraTurnSpeed * Time.deltaTime;
            animator.transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        void UpdateAnimator(float forward, float horizontal, bool grounded, bool focussed) {
            animator.SetBool("isFocussed", focussed);
            animator.SetBool("isGrounded", grounded);

            animator.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        }
    }
}