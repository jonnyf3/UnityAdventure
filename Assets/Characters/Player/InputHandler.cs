using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CameraController))]
public class InputHandler : MonoBehaviour
{
    CameraController camera = null;
    PlayerMovement character = null;
    Animator animator = null;

    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<CameraController>();
        character = GetComponent<PlayerMovement>();

        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        camera.Turn(CrossPlatformInputManager.GetAxis("CameraX"));
        camera.Elevate(CrossPlatformInputManager.GetAxis("CameraY"));

        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        Vector3 movement = v * camera.Forward + h * camera.Right;

        character.Move(movement);
        
        if (CrossPlatformInputManager.GetButtonDown("Square")) {
            animator.SetTrigger("Attack");
        }
    }
}