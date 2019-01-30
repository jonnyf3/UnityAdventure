using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;
    [SerializeField] Transform weaponHand;
    
    // Start is called before the first frame update
    void Start() {
        var weapon = Instantiate(currentWeapon.WeaponPrefab, weaponHand);
        weapon.transform.localPosition = currentWeapon.Grip.position;
        weapon.transform.localRotation = currentWeapon.Grip.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
