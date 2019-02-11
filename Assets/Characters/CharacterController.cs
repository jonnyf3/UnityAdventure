using UnityEngine;

namespace RPG.Characters
{
    public class CharacterController : MonoBehaviour
    {
        protected CharacterMovement movement = null;
        protected Health health = null;

        // Start is called before the first frame update
        void Awake() {
            health = GetComponent<Health>();
            if (!health) { health = gameObject.AddComponent<Health>(); }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}