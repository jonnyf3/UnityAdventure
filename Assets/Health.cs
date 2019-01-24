using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    public float HealthPercent {
        get { return currentHealth / maxHealth; }
    }

    // Start is called before the first frame update
    void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}