using UnityEngine;
using RPG.Characters;

namespace RPG.Combat
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        public Weapon Data { protected get; set; }

        protected Character owner;
        private void Awake() {
            owner = GetComponentInParent<Character>();
        }

        public abstract void Attack();
    }
}