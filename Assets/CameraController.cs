using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //print(arm.transform.rotation.x);
        //Mathf.Clamp(arm.rotation.eulerAngles.x, -15f, 85f);
    }

    //private void OnDrawGizmos() {
    //    Gizmos.DrawLine(transform.position, transform.position + Right);
    //    Gizmos.DrawLine(transform.position, transform.position + Forward);
    //}
}