using UnityEngine;
using RPG.Core;

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

        public void TakeDamage(float damage) {
            //NPCs do not actually take damage, but should still respond to being attacked
            animator.SetTrigger("Attacked");
        }
    }
}