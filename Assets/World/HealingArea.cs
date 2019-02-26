using UnityEngine;
using RPG.Characters;

public class HealingArea : MonoBehaviour
{
    [SerializeField] float healthPerSecond = 5f;

    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<PlayerController>()) {
            other.GetComponent<Health>().RestoreHealth(healthPerSecond * Time.deltaTime);
        }
    }
}