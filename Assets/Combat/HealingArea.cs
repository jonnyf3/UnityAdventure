using UnityEngine;
using RPG.Characters;

namespace RPG.Combat
{
    public class HealingArea : MonoBehaviour
    {
        [SerializeField] float healthPerSecond = 5f;

        private void OnTriggerStay(Collider other) {
            if (other.GetComponent<Player>()) {
                other.GetComponent<Health>().RestoreHealth(healthPerSecond * Time.deltaTime);
            }
        }
    }
}