using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Transform gripPosition;
    [SerializeField] AnimationClip attackAnimation;

    public GameObject WeaponPrefab {
        get { return weaponPrefab; }
    }

    public Transform Grip {
        get { return gripPosition; }
    }
}
