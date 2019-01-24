﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

//[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CameraController))]
public class InputHandler : MonoBehaviour
{
    CameraController camera;
    PlayerMovement character;

    // Start is called before the first frame update
    void Start() {
        camera = GetComponent<CameraController>();
        character = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        //print(CrossPlatformInputManager.GetAxis("CameraX"));
        camera.Turn(CrossPlatformInputManager.GetAxis("CameraX"));
        camera.Elevate(CrossPlatformInputManager.GetAxis("CameraY"));

        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        Vector3 movement = v * camera.Forward + h * transform.right;
        character.Move(movement);
    }
}