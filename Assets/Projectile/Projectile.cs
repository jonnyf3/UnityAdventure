﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] float speed = 4f;

    public float Speed {
        get { return speed; }
    }

    private void OnCollisionEnter(Collision collision) {
        var damageable = collision.transform.GetComponentInParent<IDamageable>();
        if (damageable != null) {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
