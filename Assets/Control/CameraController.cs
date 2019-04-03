using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;
using RPG.UI;
using RPG.Combat;

namespace RPG.Control
{
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] Transform gimbal = null;
        [SerializeField] Transform basePosition = null;
        [SerializeField] float zoomSpeed = 5f;
        [SerializeField] float minZoomDistance = 0.5f;
        private Transform arm;
        private Viewer viewer;

        private CinemachineVirtualCamera[] cameras;
        public CinemachineVirtualCamera ActiveCamera { get; private set; }

        [Header("Settings")]
        [SerializeField] float cameraSensitivity = 2f;
        [SerializeField] bool invertX = false;
        [SerializeField] bool invertY = false;

        private void Awake() {
            Assert.IsNotNull(gimbal, "Camera gimbal has not been identified!");
            arm = gimbal.transform.GetChild(0);
            cameras = arm.GetComponentsInChildren<CinemachineVirtualCamera>();
            ActiveCamera = cameras[0];

            viewer = FindObjectOfType<Viewer>();
            Assert.IsNotNull(viewer, "There is no camera Viewer to focus on");
        }

        public Vector3 LookTarget => viewer.LookTarget;
        public Vector3 Forward => Vector3.ProjectOnPlane(LookTarget - ActiveCamera.transform.position, transform.up).normalized;
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

        public void EnableFocusCamera()  => ActiveCamera = cameras[1];
        public void DisableFocusCamera() => ActiveCamera = cameras[0];

        private void Start() {
            if (Application.isPlaying) { StartCoroutine(AvoidCameraObstruction()); }
        }

        private void Update() {
            if (!Application.isPlaying) { return; }
            foreach (var camera in cameras) {
                camera.gameObject.SetActive(false);
            }
            ActiveCamera.gameObject.SetActive(true);
        }

        private IEnumerator AvoidCameraObstruction() {
            Vector3 cameraStartPos = ActiveCamera.transform.localPosition;

            Vector3 playerCentre, vectorToCam;
            while (true) {
                playerCentre = transform.position + Vector3.up;
                vectorToCam = ActiveCamera.transform.position - playerCentre;
                while (Physics.Raycast(playerCentre, vectorToCam.normalized, out RaycastHit hitInfo, vectorToCam.magnitude, ~0, QueryTriggerInteraction.Ignore)) {
                    Vector3 targetPos;
                    if (Vector3.Distance(playerCentre, hitInfo.point) >= minZoomDistance) {
                        targetPos = hitInfo.point;
                    } else {
                        targetPos = playerCentre + (minZoomDistance * vectorToCam.normalized);
                    }
                    ActiveCamera.transform.position = Vector3.Lerp(ActiveCamera.transform.position, targetPos, zoomSpeed * Time.deltaTime);

                    playerCentre = transform.position + Vector3.up;
                    vectorToCam = ActiveCamera.transform.position - playerCentre;
                    yield return new WaitForEndOfFrame();
                }
                ActiveCamera.transform.localPosition = Vector3.Lerp(ActiveCamera.transform.localPosition, cameraStartPos, zoomSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private void LateUpdate() {
            if (!Application.isPlaying || !GetComponent<Health>().IsDead) {
                gimbal.position = basePosition.position;
            }
        }
    }
}