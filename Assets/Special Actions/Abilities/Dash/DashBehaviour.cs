using System.Collections;
using UnityEngine;
using RPG.Control;

namespace RPG.Actions
{
    public class DashBehaviour : AbilityBehaviour
    {
        private float Range => (Data as DashData).range;
        private float Speed => (Data as DashData).dashSpeed;
        private GameObject GhostPrefab => (Data as DashData).prefab;
        private Material GhostMaterial => (Data as DashData).dashMaterial;
        private Material GhostBadMaterial => (Data as DashData).badMaterial;

        public override void Use() {
            StartCoroutine(Dash());
        }

        private IEnumerator Dash() {
            Vector3 moveTarget = Vector3.zero;
            Vector3 offset = (0.25f * Vector3.up);

            GameObject model = Instantiate(GhostPrefab);
            while (Input.GetButton(ControllerInput.ABILITY_BUTTON)) {
                var direction = GetDirection(Input.GetAxis(ControllerInput.MOVE_Y_AXIS),
                                             Input.GetAxis(ControllerInput.MOVE_X_AXIS));
                moveTarget = GetGroundPosition(transform.position + offset + direction * Range);

                if (moveTarget != Vector3.zero) {
                    model.transform.position = moveTarget;
                    model.GetComponent<MeshRenderer>().material = GhostMaterial;
                } else {
                    model.transform.position = transform.position + direction * Range;
                    model.GetComponent<MeshRenderer>().material = GhostBadMaterial;
                }
                model.transform.forward = direction;
                yield return new WaitForEndOfFrame();
            }

            Destroy(model);
            if (moveTarget != Vector3.zero) {
                StartCoroutine(DashToTarget(moveTarget));
                AbilityUsed();
            }
        }

        private IEnumerator DashToTarget(Vector3 position) {
            var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            var rigidBody = GetComponent<Rigidbody>();

            var startMaterial = renderer.material;
            renderer.material = GhostMaterial;
            rigidBody.detectCollisions = false;
            rigidBody.useGravity = false;

            var startPosition = transform.position;
            var vecToTarget = position - transform.position;
            var travelDistance = vecToTarget.magnitude;
            while ((transform.position - startPosition).magnitude <= travelDistance) {
                transform.position += vecToTarget.normalized * Speed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            transform.position = position;
            renderer.material = startMaterial;
            rigidBody.detectCollisions = true;
            rigidBody.useGravity = true;
        }

        private Vector3 GetDirection(float forward, float right) {
            if (forward == 0 && right == 0) { return transform.forward; }

            var camera = GetComponent<CameraController>();
            return (forward * camera.Forward + right * camera.Right).normalized;
        }

        private Vector3 GetGroundPosition(Vector3 target) {
            Vector3 offset = (Vector3.up * 0.25f);  //offset to avoid raycasting at ground level
            if (Physics.Raycast(transform.position + offset, target - transform.position, out RaycastHit hitInfo, Range, ~0, QueryTriggerInteraction.Ignore)) {
                var obstruction = hitInfo.transform.gameObject;
                if (obstruction.isStatic) {
                    if (obstruction == Terrain.activeTerrain.gameObject) {
                        target = hitInfo.point + offset;  //offset so that ground-finding raycast will hit ground
                    } else {
                        //set target just short of any other static obstruction
                        var vecToTarget = hitInfo.point - transform.position;
                        var reducedVecToTarget = vecToTarget - (2 * GetComponent<CapsuleCollider>().radius * vecToTarget.normalized);
                        
                        target = transform.position + reducedVecToTarget;
                    }
                }
            }

            return FindGroundAboveOrBelow(target);
        }

        private Vector3 FindGroundAboveOrBelow(Vector3 point) {
            float maxUpAngle = 20f;
            float maxDownAngle = 45f;
            float maxUpDistance   = (transform.position - point).magnitude * Mathf.Tan(maxUpAngle * Mathf.Deg2Rad);
            float maxDownDistance = (transform.position - point).magnitude * Mathf.Tan(maxDownAngle * Mathf.Deg2Rad);

            //find ground below target
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hitInfo,
                                maxDownDistance, ~0, QueryTriggerInteraction.Ignore)) {
                return hitInfo.point;
            }
            //find ground above target
            else if (Physics.Raycast(point + (Vector3.up * maxUpDistance), Vector3.down, out hitInfo,
                                     maxUpDistance, ~0, QueryTriggerInteraction.Ignore)) {
                return hitInfo.point;
            }
            else {
                return Vector3.zero;
            }
        }
    }
}