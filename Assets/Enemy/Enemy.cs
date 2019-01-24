using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float chaseRadius = 10f;

    [SerializeField] Projectile projectile = null;
    [SerializeField] Transform projectileSocket = null;

    private GameObject player = null;
    private AICharacterControl ai = null;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        ai = GetComponentInChildren<AICharacterControl>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= chaseRadius) {
            ai.SetTarget(player.transform);
        } else {
            ai.SetTarget(transform);
        }

        if (distanceToPlayer <= attackRadius) {
            SpawnProjectile();
            ai.SetTarget(transform);
        }
    }

    private void SpawnProjectile() {
        Projectile newProjectile = Instantiate(projectile, projectileSocket.transform.position, Quaternion.identity);

        Vector3 unitVectorToPlayer = (player.transform.position - projectileSocket.transform.position).normalized;
        float projectileSpeed = newProjectile.Speed;
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
    }

    private void LateUpdate() {
        transform.position += ai.transform.localPosition; ;
        ai.transform.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
