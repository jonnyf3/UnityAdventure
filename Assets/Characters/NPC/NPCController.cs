﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.Core;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class NPCController : MonoBehaviour, IDamageable
    {
        [SerializeField] string name;
        [SerializeField] float activationRadius = 6f;
        private Animator animator = null;
        private Text displayText = null;

        // Start is called before the first frame update
        void Start() {
            animator = GetComponentInChildren<Animator>();

            var ui = GetComponentInChildren<CharacterUI>();
            displayText = ui.GetComponentInChildren<Text>();
            Assert.IsNotNull(displayText);
            DeactivateUI();

            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = activationRadius;

            var viewer = Camera.main.GetComponent<Viewer>();
            viewer.onChangedFocus += DeactivateUI;
        }

        private void LateUpdate() {
            transform.position = animator.transform.position;
            animator.transform.localPosition = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.GetComponent<Player>()) {
                animator.SetBool("PlayerInRange", true);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.GetComponent<Player>()) {
                animator.SetBool("PlayerInRange", false);
            }
        }

        public void ActivateUI() {
            displayText.text = name;
        }
        private void DeactivateUI() {
            displayText.text = "";
        }

        public void TakeDamage(float damage) {
            //NPCs do not actually take damage, but should still respond to being attacked
            animator.SetTrigger("Attacked");
        }
    }
}