using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.UI;
using RPG.Characters;

namespace RPG.Actions
{
    public class DashBehaviour : AbilityBehaviour
    {
        private float Range => (Data as DashData).range;
        private float Speed => (Data as DashData).dashSpeed;
        private GameObject GhostPrefab => (Data as DashData).prefab;
        private Material GhostMaterial => (Data as DashData).dashMaterial;
        private Material GhostBadMaterial => (Data as DashData).badMaterial;

        private Vector3 dashTarget;
        Vector3 raycastOffset = (0.25f * Vector3.up);

        public override void Use() {
            StartCoroutine(Dash());
        }

        private IEnumerator Dash() {
            GameObject model = Instantiate(GhostPrefab);
            
            while (Input.GetButton(ControllerInput.ABILITY_BUTTON)) {
                if (GetValidDashDestination()) {
                    model.transform.position = dashTarget;
                    model.GetComponent<MeshRenderer>().material = GhostMaterial;
                } else {
                    model.transform.position = GetLookTarget();
                    model.GetComponent<MeshRenderer>().material = GhostBadMaterial;
                }
                model.transform.forward = transform.forward;
                yield return new WaitForEndOfFrame();
            }

            Destroy(model);
            if (GetValidDashDestination()) {
                StartCoroutine(DashToTarget(dashTarget));
                AbilityUsed();
            }
        }

        private IEnumerator DashToTarget(Vector3 position) {
            var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            var rigidBody = GetComponent<Rigidbody>();
            var animator = GetComponent<Animator>();

            var startMaterial = renderer.material;
            renderer.material = GhostMaterial;
            rigidBody.detectCollisions = false;
            rigidBody.useGravity = false;
            animator.applyRootMotion = false;

            var animStartTime = Time.time;
            var startPosition = transform.position;
            var vecToTarget = position - transform.position;
            var travelDistance = vecToTarget.magnitude;
            while ((transform.position - startPosition).magnitude <= travelDistance) {
                transform.position += vecToTarget.normalized * Speed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            //prevent moving beyond target because of animator root motion
            do {
                transform.position = position;
                rigidBody.velocity = Vector3.zero;
                yield return new WaitForEndOfFrame();
            } while (Time.time - animStartTime <= Data.AnimClip.length);

            renderer.material = startMaterial;
            rigidBody.detectCollisions = true;
            rigidBody.useGravity = true;
            animator.applyRootMotion = true;
        }
        
        private bool GetValidDashDestination() {
            var lookTarget = GetLookTarget();
            var direction = (lookTarget - (transform.position + raycastOffset)).normalized;
            var target = (transform.position + raycastOffset) + direction * Range;
            if (Physics.Raycast(transform.position + raycastOffset, direction, out RaycastHit hitInfo, Range, ~0, QueryTriggerInteraction.Ignore) && !hitInfo.transform.GetComponent<Character>()) {
                //Dash direction is obstructed
                if (Mathf.Abs(Vector3.SignedAngle(hitInfo.normal, Vector3.up, Vector3.left)) > 60f) {
                    //looking at a wall, try to dash up
                    if (FindGroundAbove(hitInfo.point + (direction * 0.2f))) {
                        return true;
                    } else {
                        //set target just short of any other static obstruction
                        target = hitInfo.point - (2 * GetComponent<CapsuleCollider>().radius) * direction;
                    }
                } else {
                    //looking at "flat" ground (potential valid target)
                    target = hitInfo.point + raycastOffset;
                }
            }
            
            //from target point (in open space), check back along direction for a point where FindGroundBelow is in range
            while (!FindGroundBelow(target) && Vector3.Distance(target, transform.position) > 0.5f) {
                target -= direction * 0.1f;
            }
            return FindGroundBelow(target);
        }

        private Vector3 GetLookTarget() {
            if (Input.GetButton(ControllerInput.FOCUS_BUTTON)) {
                var viewerTarget = FindObjectOfType<Viewer>().LookTarget;
                if (Vector3.Distance(transform.position, viewerTarget) > Range) {
                    var lookVector = (viewerTarget - transform.position).normalized;
                    viewerTarget = transform.position + (lookVector.normalized * Range);
                }
                return viewerTarget;
            } else {
                return transform.position + raycastOffset + (transform.forward * Range);
            }
        }

        private bool FindGroundAbove(Vector3 point) {
            float maxUpDistance = Mathf.Sqrt(Range*Range - (transform.position - point).sqrMagnitude);

            if (Physics.Raycast(point + (Vector3.up * maxUpDistance), Vector3.down, out RaycastHit hitInfo,
                                maxUpDistance, ~0, QueryTriggerInteraction.Ignore)) {
                dashTarget = hitInfo.point;
                return true;
            }
            return false;
        }

        private bool FindGroundBelow(Vector3 point) {
            float maxDownDistance = Mathf.Sqrt(Range * Range - (transform.position - point).sqrMagnitude);
            
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hitInfo,
                                maxDownDistance, ~0, QueryTriggerInteraction.Ignore)) {
                dashTarget = hitInfo.point;
                return true;
            }
            return false;
        }
    }
}