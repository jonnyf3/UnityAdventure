﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class NPCController : AICharacter
    {
        [Header("NPC")]
        [SerializeField] new string name = "";
        [SerializeField] float activationRadius = 6f;
        private Text displayText;

        private PlayerController player;
        private bool playerInRange = false;
        
        protected override void Start() {
            base.Start();

            var displayText = ui.GetComponentInChildren<Text>();
            Assert.IsNotNull(displayText);
            displayText.text = name;

            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = activationRadius;

            player = FindObjectOfType<PlayerController>();
        }

        protected override void Update() {
            //Don't do movement from base update
            //TODO NPC animator doesn't contain required parameters
            if (playerInRange) {
                if (patrolPath) { StartCoroutine(Patrol()); }
                transform.LookAt(player.transform, Vector3.up);
            }
            else { StopAllCoroutines(); }

            agent.transform.localPosition = Vector3.zero;
            return;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.GetComponent<PlayerController>()) {
                playerInRange = true;
                animator.SetBool("PlayerInRange", true);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.GetComponent<PlayerController>()) {
                playerInRange = false;
                animator.SetBool("PlayerInRange", false);
            }
        }

        public override void Die() {
            base.Die();
            health.RestoreHealth(1f);
        }
    }
}