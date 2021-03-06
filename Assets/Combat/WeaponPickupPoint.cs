﻿using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.Combat
{
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        //[SerializeField] AudioClip soundEffect = null; //TODO add WeaponData.PickupSound?

        private float spinsPerSecond = 0.5f;

        // Update is called once per frame
        void Update() {
            if (!Application.isPlaying) {
                DestroyChildren();
                InstantiateWeapon();
            }
            else {
                transform.Rotate(Vector3.up, 360f * spinsPerSecond * Time.deltaTime, Space.World);
            }
        }

        void InstantiateWeapon() {
            Assert.IsNotNull(weapon, "No weapon assigned to weapon pickup point");
            var weaponObj = Instantiate(weapon.prefab, gameObject.transform);
            weaponObj.transform.localPosition = Vector3.zero;
        }

        private void DestroyChildren() {
            foreach (Transform child in transform) {
                DestroyImmediate(child.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.isTrigger) { return; }

            var player = other.gameObject.GetComponent<Player>();
            if (player) {
                player.GiveWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}