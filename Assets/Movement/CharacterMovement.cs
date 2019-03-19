using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        private Animator animator;
        
        [Header("Moving")]
        [SerializeField] float moveSpeedMultiplier = 1.25f;
        [SerializeField] float extraTurnSpeed = 360f;
        [SerializeField][Range(0.1f, 1f)] float animatorForwardCap = 1f;  //Limit the maximum "forward" amount sent to the animator
        
        [Header("Ground Check")]
        [SerializeField] float groundCheckDistance = 0.3f;
        [SerializeField][Range(1f, 4f)] float gravityMultiplier = 2f;  //TODO re-use in FallingState? Or delete

        public float AnimatorForwardCap => animatorForwardCap;
        public float GroundCheckDistance => groundCheckDistance;
        public float MoveSpeed => moveSpeedMultiplier;
        
        void Start() {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator, "No animator found on " + gameObject);
            animator.applyRootMotion = true;
            animator.speed = moveSpeedMultiplier;
        }

        public void Move(Vector3 direction, float maxForward) {
            var localMovement = transform.InverseTransformDirection(direction);

            //always limited by specified forward cap
            if (maxForward >= animatorForwardCap) { maxForward = animatorForwardCap; }
            float forward = Mathf.Clamp(localMovement.z, -maxForward, maxForward);

            //Ensure large turning when desired direction is directly backwards
            float horizontal = localMovement.x;
            if (forward < 0 && !animator.GetBool("isFocussed")) { horizontal += Mathf.Abs(forward); }

            ApplyExtraTurnRotation(horizontal);
            UpdateAnimator(forward, horizontal);
        }

        public void TurnTowards(Vector3 vectorToTarget) {
            float requiredTurn = Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(vectorToTarget, Vector3.up), Vector3.up);
            if (vectorToTarget.magnitude > 1f && Mathf.Abs(requiredTurn) >= 10f) {
                UpdateAnimator(0f, Mathf.Sign(requiredTurn));
            } else if (Mathf.Abs(requiredTurn) >= 45f) {
                UpdateAnimator(0f, Mathf.Sign(requiredTurn));
            } else {
                UpdateAnimator(0f, 0f);
            }
        }

        void ApplyExtraTurnRotation(float turn) {
            float rotation = turn * extraTurnSpeed * Time.deltaTime;
            animator.transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        void UpdateAnimator(float forward, float horizontal) {
            animator.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        }
    }
}