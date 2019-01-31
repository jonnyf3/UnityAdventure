using UnityEngine;

namespace RPG.CameraUI
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform gimbal;
        [SerializeField] Transform arm;

        [SerializeField] float cameraSensitivity = 2f;
        [SerializeField] bool invertX = true;
        [SerializeField] bool invertY = true;

        public Vector3 Forward {
            get { return gimbal.forward; }
        }

        public Vector3 Right {
            get { return gimbal.right; }
        }

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

        //private void OnDrawGizmos() {
        //    Gizmos.DrawLine(transform.position, transform.position + Right);
        //    Gizmos.DrawLine(transform.position, transform.position + Forward);
        //}
    }
}