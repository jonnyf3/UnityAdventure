using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] float speed = 4f;

    public float Speed {
        get { return speed; }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        var damageable = other.GetComponentInParent<IDamageable>();
        if(damageable != null) {
            damageable.TakeDamage(damage);
        }
    }
}
