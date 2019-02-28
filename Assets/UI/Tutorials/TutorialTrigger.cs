using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.UI
{
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] Tutorial tutorial = null;

        private void OnTriggerEnter(Collider other) {
            Assert.IsNotNull(tutorial, "No tutorial has been assigned to this trigger");

            if(tutorial && other.GetComponent<Player>()) {
                FindObjectOfType<HUD>().ShowTutorial(tutorial);
                Destroy(gameObject);
            }
        }
    }
}