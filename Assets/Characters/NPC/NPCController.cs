using UnityEngine;
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
        
        protected override void Start() {
            base.Start();

            var displayText = ui.GetComponentInChildren<Text>();
            Assert.IsNotNull(displayText);
            displayText.text = name;

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
    }
}