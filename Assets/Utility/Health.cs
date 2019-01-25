using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider healthBar = null;
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

        if (healthBar) { healthBar.value = HealthPercent; }

        if (currentHealth <= 0) { Destroy(gameObject); }
    }
}