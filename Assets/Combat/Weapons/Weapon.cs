using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Combat
{
    public abstract class Weapon : ScriptableObject
    {
        private enum Handedness { Left, Right }

        [Header("General")]
        public GameObject prefab = null;
        public Sprite     sprite = null;

        [Header("Grip")]
        [SerializeField] Handedness gripHand = default;
        [SerializeField] Transform  gripPosition = null;

        [Header("Animation")]
        [SerializeField] AnimationClip attackAnimation = null;
        [SerializeField] float animationSpeedMultiplier = 1f;

        [Header("Damage")]
        public float damage = 20f;
        public float range  = 5f;

        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                attackAnimation.events = new AnimationEvent[0];
                return attackAnimation;
            }
        }
        public float AnimationSpeed => animationSpeedMultiplier;
        
        public GameObject Setup(Transform leftHand, Transform rightHand) {
            Transform hand = null;
            if (gripHand == Handedness.Left)  { hand = leftHand; }
            if (gripHand == Handedness.Right) { hand = rightHand; }
            Assert.IsNotNull(hand, "Must specify a preferred hand for the current weapon!");

            var weaponObj = Instantiate(prefab, hand);
            weaponObj.transform.localPosition = gripPosition.position;
            weaponObj.transform.localRotation = gripPosition.rotation;

            SetupBehaviour(weaponObj);
            return weaponObj;
        }
        protected abstract void SetupBehaviour(GameObject obj);
    }
}