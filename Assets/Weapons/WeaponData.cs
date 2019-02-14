using UnityEngine;

namespace RPG.Weapons
{
    public abstract class WeaponData : ScriptableObject
    {
        public enum Handedness { Left, Right }

        [Header("General")]
        [SerializeField] Sprite sprite = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Handedness gripHand = default;
        [SerializeField] Transform gripPosition = null;
        [SerializeField] AnimationClip attackAnimation = null;
        
        [Header("Damage")]
        [SerializeField] float damage = 20f;
        [SerializeField] float attackRange = 5f;

        public GameObject WeaponPrefab { get { return weaponPrefab; } }
        public Transform  Grip         { get { return gripPosition; } }
        public Handedness PreferredHand { get { return gripHand; } }
        public Sprite     Sprite       { get { return sprite; } }

        public float Damage      { get { return damage; } }
        public float AttackRange { get { return attackRange; } }

        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                attackAnimation.events = new AnimationEvent[0];

                return attackAnimation;
            }
        }

        WeaponBehaviour behaviour;
        public GameObject SetupWeaponOnCharacter(GameObject owner, Transform hand) {
            var weaponObj = Instantiate(weaponPrefab, hand);

            weaponObj.transform.localPosition = gripPosition.position;
            weaponObj.transform.localRotation = gripPosition.rotation;

            behaviour = GetWeaponBehaviour(weaponObj);
            behaviour.Data = this;
            behaviour.SetOwner(owner);

            return weaponObj;
        }

        protected abstract WeaponBehaviour GetWeaponBehaviour(GameObject obj);

        public void Attack() { behaviour.Attack(); }
    }
}