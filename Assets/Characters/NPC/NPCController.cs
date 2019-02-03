using UnityEngine;
using RPG.Core;
using RPG.CameraUI;
using System;

namespace RPG.Characters
{
    public class NPCController : MonoBehaviour, IDamageable
    {
        [SerializeField] float activationRadius = 6f;
        private Animator animator = null;

        // Start is called before the first frame update
        void Start() {
            animator = GetComponentInChildren<Animator>();

            var sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = activationRadius;

            var viewer = Camera.main.GetComponent<Viewer>();
            viewer.onLookingAtNPC += OnBeingLookedAt;
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

        private void OnBeingLookedAt() {
            print("Looking at " + gameObject);
        }

        public void TakeDamage(float damage) {
            //NPCs do not actually take damage, but should still respond to being attacked
            animator.SetTrigger("Attacked");
        }
    }
}