using UnityEngine;

namespace RPG.Actions
{
    public abstract class Interactable : MonoBehaviour
    {
        public delegate void OnInteraction();
        public event OnInteraction onInteraction;

        protected void Interact() => onInteraction();
    }

}