using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.UI;
using RPG.Combat;

namespace RPG.Control
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] Transform gimbal = null;
        [SerializeField] Transform basePosition = null;
        [SerializeField] float zoomSpeed = 5f;
        [SerializeField] float minZoomDistance = 0.5f;
        private Transform arm;
        private Transform cam;
        private Viewer viewer;

        [Header("Settings")]
        [SerializeField] float cameraSensitivity = 2f;
        [SerializeField] bool invertX = false;
        [SerializeField] bool invertY = false;

        private void Awake() {
            Assert.IsNotNull(gimbal, "Camera gimbal has not been identified!");
            arm = gimbal.transform.GetChild(0);
            cam = arm.GetChild(0);

            viewer = FindObjectOfType<Viewer>();
            Assert.IsNotNull(viewer, "There is no camera Viewer to focus on");
        }

        public Vector3 Forward => Vector3.ProjectOnPlane(viewer.LookTarget - cam.position, transform.up).normalized;
        public Vector3 Right   => Vector3.Cross(transform.up, Forward);

        public void Turn(float degrees) {
            int invertFactor = invertX ? -1 : 1;
            gimbal.Rotate(0f, degrees * invertFactor * cameraSensitivity, 0f);
        }

        public void Elevate(float degrees) {
            int invertFactor = invertY ? -1 : 1;
            arm.Rotate(degrees * invertFactor * cameraSensitivity, 0f, 0f);

            //Limit camera elevation to stop upside-down camera/weird movement
            var currentX = arm.transform.eulerAngles.x;
            if (currentX > 180) { currentX -= 360; }
            var restrictedX = Mathf.Clamp(currentX, -25f, 85f);
            arm.transform.eulerAngles = new Vector3(restrictedX, arm.transform.eulerAngles.y, arm.transform.eulerAngles.z);
        }

        private void Start() {
            if (Application.isPlaying) { StartCoroutine(AvoidCameraObstruction()); }
        }

        private IEnumerator AvoidCameraObstruction() {
            Vector3 cameraStartPos = cam.transform.localPosition;

            Vector3 playerCentre, vectorToCam;
            while (true) {
                playerCentre = transform.position + Vector3.up;
                vectorToCam = Camera.main.transform.position - playerCentre;
                while (Physics.Raycast(playerCentre, vectorToCam.normalized, out RaycastHit hitInfo, vectorToCam.magnitude, ~0, QueryTriggerInteraction.Ignore)) {
                    Vector3 targetPos;
                    if (Vector3.Distance(playerCentre, hitInfo.point) >= minZoomDistance) {
                        targetPos = hitInfo.point;
                    } else {
                        targetPos = playerCentre + (minZoomDistance * vectorToCam.normalized);
                    }
                    cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, zoomSpeed * Time.deltaTime);

                    playerCentre = transform.position + Vector3.up;
                    vectorToCam = cam.transform.position - playerCentre;
                    yield return new WaitForEndOfFrame();
                }
                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraStartPos, zoomSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private void LateUpdate() {
            if (!GetComponent<Health>().IsDead) { gimbal.position = basePosition.position; }
        }
    }
}