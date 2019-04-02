using System;
using UnityEngine;

namespace RPG.Actions
{
    public abstract class Interactable : MonoBehaviour
    {
        public event Action onInteraction;
        protected void Interact() => onInteraction();
    }

}