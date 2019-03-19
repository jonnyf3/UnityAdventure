﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;
using RPG.Actions;
using RPG.UI;
using RPG.Control;

namespace RPG.States
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(CombatSystem))]
    [RequireComponent(typeof(SpecialAbilities))]
    public class ControlledState : State
    {
        //required components
        CombatSystem     combat;
        WeaponSystem     weapons;
        SpecialAbilities abilities;
        new CameraController camera;
        Viewer           viewer;
        Animator         animator;

        private Transform projectileSpawn => (character as Player).projectileSpawn;
        private Transform abilitySpawn    => (character as Player).abilitySpawn;

        private float colliderOriginalHeight;
        private Vector3 colliderOriginalCenter;

        public const string ANIMATOR_ROLL_PARAM = "onRoll";

        private void Awake() {
            var collider = GetComponent<CapsuleCollider>();
            colliderOriginalHeight = collider.height;
            colliderOriginalCenter = collider.center;
        }

        protected override void Start() {
            base.Start();
            Assert.IsNotNull(character as Player, "ControlledState should only be entered by a Player character");

            animator = GetComponent<Animator>();
            combat = GetComponent<CombatSystem>();
            weapons = GetComponent<WeaponSystem>();
            abilities = GetComponent<SpecialAbilities>();

            camera = GetComponent<CameraController>();
            viewer = FindObjectOfType<Viewer>();
            Assert.IsNotNull(viewer, "There is no camera Viewer to focus on");
        }

        private void Update() {
            if (!character.IsOnGround) { character.SetState<FallingState>(); return; }

            ProcessMovement(Input.GetAxis(ControllerInput.MOVE_Y_AXIS),
                            Input.GetAxis(ControllerInput.MOVE_X_AXIS));
            
            ProcessCameraMovement(Input.GetAxis(ControllerInput.CAMERA_X_AXIS),
                                  Input.GetAxis(ControllerInput.CAMERA_Y_AXIS));

            if (Input.GetButtonDown(ControllerInput.FOCUS_BUTTON)) {
                StartCoroutine(Focus());
            }

            if (Input.GetButtonDown(ControllerInput.ATTACK_BUTTON)) {
                AlignSpawnPoint(projectileSpawn);
                combat.Attack();
            }
            if (Input.GetButtonDown(ControllerInput.ABILITY_BUTTON)) {
                AlignSpawnPoint(abilitySpawn);
                abilities.Use();
            }
            if (Input.GetButtonDown(ControllerInput.ROLL_BUTTON)) {
                StartCoroutine(Roll());
            }

            ProcessWeaponToggle();
        }

        private void ProcessMovement(float forward, float right) {
            // Get player controller input direction relative to camera direction
            var cameraRelative = forward * camera.Forward + right * camera.Right;
            character.Move(transform.position + cameraRelative.normalized);
        }

        private void ProcessCameraMovement(float rotation, float elevation) {
            camera.Turn(rotation);
            camera.Elevate(elevation);
        }

        private IEnumerator Focus() {
            character.Focus(true);

            var currentFOV = Camera.main.fieldOfView;
            Camera.main.fieldOfView = 40;

            //Focus on viewpoint target while trigger is held down
            while (Input.GetButton(ControllerInput.FOCUS_BUTTON)) {
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                yield return new WaitForEndOfFrame();
            }

            Camera.main.fieldOfView = currentFOV;
            character.Focus(false);
        }

        private IEnumerator Roll() {
            animator.SetTrigger(ANIMATOR_ROLL_PARAM);

            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight / 2f;
            collider.center -= new Vector3(0, colliderOriginalHeight / 4f, 0f);
            yield return new WaitForSeconds(0.8f);

            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }

        private void ProcessWeaponToggle() {
            if (ControllerInput.GetVerticalButtonsDown()) {
                var verticalButtonDirection = (int)Mathf.Sign(Input.GetAxis(ControllerInput.DPAD_Y_AXIS));
                weapons.CycleWeapon(verticalButtonDirection);
            }
            if (ControllerInput.GetHorizontalButtonsDown()) {
                var horizontalButtonDirection = (int)Mathf.Sign(Input.GetAxis(ControllerInput.DPAD_X_AXIS));
                abilities.CycleAbilities(horizontalButtonDirection);
            }
        }

        private void AlignSpawnPoint(Transform spawnPoint) {
            if (!spawnPoint) { return; }

            if (animator.GetBool("isFocussed")) {
                spawnPoint.LookAt(viewer.LookTarget);
            } else {
                spawnPoint.forward = transform.forward;
            }
        }

        private void OnDestroy() {
            //catch in case of state exit mid-roll
            var collider = GetComponent<CapsuleCollider>();
            collider.height = colliderOriginalHeight;
            collider.center = colliderOriginalCenter;
        }
    }
}