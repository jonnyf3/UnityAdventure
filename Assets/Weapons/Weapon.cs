﻿using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class Weapon : ScriptableObject
    {
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Transform gripPosition = null;
        [SerializeField] AnimationClip attackAnimation = null;
        [SerializeField] float damage;
        [SerializeField] float range;

        public GameObject WeaponPrefab {
            get { return weaponPrefab; }
        }

        public Transform Grip {
            get { return gripPosition; }
        }

        public AnimationClip AnimClip {
            get {
                //Remove any animation events (imported via asset pack) from clip
                attackAnimation.events = new AnimationEvent[0];

                return attackAnimation;
            }
        }

        public float Damage {
            get { return damage; }
        }
        public float Range {
            get { return range; }
        }
    }
}