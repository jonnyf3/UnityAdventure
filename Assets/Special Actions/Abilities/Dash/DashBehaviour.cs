using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.UI;

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

            Vector3 lookTarget = GetLookTarget();
            while (Input.GetButton(ControllerInput.ABILITY_BUTTON)) {
                lookTarget = GetLookTarget();

                if (GetValidDashDestination(lookTarget - transform.position)) {
                    model.transform.position = dashTarget;
                    model.GetComponent<MeshRenderer>().material = GhostMaterial;
                } else {
                    model.transform.position = lookTarget;
                    model.GetComponent<MeshRenderer>().material = GhostBadMaterial;
                }
                model.transform.forward = transform.forward;
                yield return new WaitForEndOfFrame();
            }

            Destroy(model);
            if (GetValidDashDestination(lookTarget - transform.position)) {
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

        private bool GetValidDashDestination(Vector3 direction) {
            Vector3 target = transform.position + (direction * Range);
            if (Physics.Raycast(transform.position + raycastOffset, direction, out RaycastHit hitInfo, Range, ~0, QueryTriggerInteraction.Ignore)) {
                //Dash direction is obstructed
                var obstruction = hitInfo.transform.gameObject;
                if (obstruction.isStatic) {
                    if (obstruction == Terrain.activeTerrain.gameObject) {
                        target = hitInfo.point + raycastOffset;  //offset so that ground-finding raycast will hit ground
                    } else {
                        if (FindGroundAbove(hitInfo.point + (direction.normalized * 0.2f))) {
                            return true;
                        } else {
                            //set target just short of any other static obstruction
                            target = hitInfo.point - (2 * GetComponent<CapsuleCollider>().radius)* direction.normalized;
                        }
                    }

                    if (FindGroundAbove(target) || FindGroundBelow(target)) { return true; }
                }
            }

            //no obstruction; check back along direction for a point where FindGroundBelow is in range
            while (!FindGroundBelow(target) && Vector3.Distance(target, transform.position) > 0.5f) {
                target -= direction * 0.2f;
            }
            return FindGroundBelow(target);
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
            
            Debug.DrawLine(point, point + Vector3.down * maxDownDistance, Color.red);
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hitInfo,
                                maxDownDistance, ~0, QueryTriggerInteraction.Ignore)) {
                dashTarget = hitInfo.point;
                return true;
            }
            return false;
        }
    }
}