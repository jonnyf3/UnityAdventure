using UnityEngine;

namespace RPG.Combat
{
    public abstract class WeaponData : ScriptableObject
    {
        public enum Handedness { Left, Right }

        [Header("General")]
        [SerializeField] Sprite sprite = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Handedness gripHand = default;
        [SerializeField] Transform gripPosition = null;

        [Header("Animation")]
        [SerializeField] AnimationClip attackAnimation = null;
        [SerializeField] float animationSpeedMultiplier = 1f;

        [Header("Damage")]
        [SerializeField] float damage = 20f;
        [SerializeField] float attackRange = 5f;

        public GameObject WeaponPrefab => weaponPrefab;
        public Transform  Grip         => gripPosition;
        public Handedness PreferredHand => gripHand;
        public Sprite     Sprite       => sprite;

        public float Damage      => damage;
        public float AttackRange => attackRange;
        public float AnimationSpeed => animationSpeedMultiplier;

        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                attackAnimation.events = new AnimationEvent[0];

                return attackAnimation;
            }
        }


        public GameObject SetupWeaponOnCharacter(GameObject owner, Transform hand) {
            var weaponObj = Instantiate(weaponPrefab, hand);

            weaponObj.transform.localPosition = gripPosition.position;
            weaponObj.transform.localRotation = gripPosition.rotation;

            var behaviour = GetWeaponBehaviour(weaponObj);
            behaviour.Data = this;
            behaviour.SetOwner(owner);

            return weaponObj;
        }

        protected abstract WeaponBehaviour GetWeaponBehaviour(GameObject obj);
    }
}