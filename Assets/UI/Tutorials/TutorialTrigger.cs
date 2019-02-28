using UnityEngine;
using RPG.Characters;

namespace RPG.UI
{
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] Tutorial tutorial = null;

        private void OnTriggerEnter(Collider other) {
            if(other.GetComponent<Player>()) {
                FindObjectOfType<HUD>().ShowTutorial(tutorial);
                Destroy(gameObject);
            }
        }
    }
}