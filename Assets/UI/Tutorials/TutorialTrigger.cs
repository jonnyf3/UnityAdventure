using UnityEngine;
using RPG.Characters;

namespace RPG.UI
{
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] Tutorial tutorial = null;

        private void OnTriggerEnter(Collider other) {
            if(other.GetComponent<Player>()) {
                print(tutorial.description);
                //TODO show tutorial UI
                Destroy(gameObject);
            }
        }
    }
}