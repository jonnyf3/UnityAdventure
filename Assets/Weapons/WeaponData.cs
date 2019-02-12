using UnityEngine;

namespace RPG.Weapons
{
    public abstract class WeaponData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] Sprite sprite = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Transform gripPosition = null;
        [SerializeField] AnimationClip attackAnimation = null;

        [Header("Damage")]
        [SerializeField] float damage = 20f;

        public GameObject WeaponPrefab { get { return weaponPrefab; } }
        public Transform  Grip         { get { return gripPosition; } }
        public Sprite     Sprite       { get { return sprite; } }

        public float Damage { get { return damage; } }

        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                attackAnimation.events = new AnimationEvent[0];

                return attackAnimation;
            }
        }

        WeaponBehaviour behaviour;
        public void SetupWeapon(GameObject weaponObj, GameObject owner) {
            weaponObj.transform.localPosition = gripPosition.position;
            weaponObj.transform.localRotation = gripPosition.rotation;

            behaviour = GetWeaponBehaviour(weaponObj);
            behaviour.Data = this;
            behaviour.SetOwner(owner);
        }

        protected abstract WeaponBehaviour GetWeaponBehaviour(GameObject obj);

        public void Attack() { behaviour.Attack(); }
    }
}