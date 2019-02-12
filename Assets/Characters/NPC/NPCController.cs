using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class NPCController : Character
    {
        [Header("NPC")]
        [SerializeField] new string name = "";
        [SerializeField] float activationRadius = 6f;
        private Text displayText = null;

        // Start is called before the first frame update
        protected override void Start() {
            base.Start();

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
    }
}