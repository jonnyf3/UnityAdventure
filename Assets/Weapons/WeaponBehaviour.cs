using UnityEngine;

namespace RPG.Weapons
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        public abstract void Attack();

        public WeaponData Data { protected get; set; }

        protected GameObject owner;
        public void SetOwner(GameObject owner) { this.owner = owner; }
    }
}