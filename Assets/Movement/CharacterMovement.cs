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
        public bool Focussed { get; set; }

        [Header("Ground Check")]
        [SerializeField] float groundCheckDistance = 0.3f;
        [SerializeField][Range(1f, 4f)] float gravityMultiplier = 2f;

        public delegate void OnLeftGround();
        public event OnLeftGround onLeftGround;
        public bool IsOnGround => Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, groundCheckDistance);

        public float GroundCheckDistance => groundCheckDistance;
        public float MoveSpeed => moveSpeedMultiplier;

        private float baseAnimatorForwardCap;
        public void SetAnimatorForwardCap(float value) { animatorForwardCap = value; }
        public void ResetAnimatorForwardCap() { animatorForwardCap = baseAnimatorForwardCap; }

        void Start() {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator, "No animator found on " + gameObject);
            animator.applyRootMotion = true;
            animator.speed = moveSpeedMultiplier;

            baseAnimatorForwardCap = animatorForwardCap;
        }

        private void Update() {
            if (!IsOnGround){ onLeftGround?.Invoke(); }
        }

        public void Move(Vector3 direction) {
            var localMovement = transform.InverseTransformDirection(direction);
            
            float forward = Mathf.Clamp(localMovement.z, -animatorForwardCap, animatorForwardCap);
            //Ensure large turning when desired direction is directly backwards
            float horizontal = localMovement.x;
            if (forward < 0 && !Focussed) { horizontal += Mathf.Abs(forward); }

            ApplyExtraTurnRotation(horizontal);
            UpdateAnimator(forward, horizontal, Focussed);
        }

        public void TurnTowards(Transform target) {
            Vector3 vectorToTarget = target.position - transform.position;

            float requiredTurn = Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(vectorToTarget, Vector3.up), Vector3.up);
            if (vectorToTarget.magnitude > 1f && Mathf.Abs(requiredTurn) >= 10f) {
                UpdateAnimator(0f, Mathf.Sign(requiredTurn), false);
            } else if (Mathf.Abs(requiredTurn) >= 45f) {
                UpdateAnimator(0f, Mathf.Sign(requiredTurn), false);
            } else {
                UpdateAnimator(0f, 0f, false);
            }
        }

        void ApplyExtraTurnRotation(float turn) {
            float rotation = turn * extraTurnSpeed * Time.deltaTime;
            animator.transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        void UpdateAnimator(float forward, float horizontal, bool focussed) {
            animator.SetBool("isFocussed", focussed);

            animator.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        }
    }
}