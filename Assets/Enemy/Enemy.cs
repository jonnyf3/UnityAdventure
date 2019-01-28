using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float shotsPerSecond = 1f;
    [SerializeField] float chaseRadius = 10f;
    [SerializeField] float turnSpeed = 2f;

    [SerializeField] Projectile projectile = null;
    [SerializeField] Transform projectileSocket = null;

    private GameObject player = null;
    private AICharacterControl ai = null;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        ai = GetComponentInChildren<AICharacterControl>();
    }

    private void LateUpdate() {
        transform.position += ai.transform.localPosition; ;
        ai.transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        //begin attacking when in range
        if (distanceToPlayer <= attackRadius) {
            LookTowardsPlayer();
            if (!isAttacking) {
                InvokeRepeating("FireProjectile", 0f, 1f / shotsPerSecond);
                isAttacking = true;
            }
        }
        //stop attacking if out of range
        if (distanceToPlayer > attackRadius && isAttacking) {
            CancelInvoke();
            isAttacking = false;
        }

        if (distanceToPlayer <= chaseRadius) {
            ai.SetTarget(player.transform);
        }
        else ai.SetTarget(transform);
    }

    private void LookTowardsPlayer() {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        Vector3 rotatedForward = Vector3.RotateTowards(transform.forward, vectorToPlayer, turnSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(rotatedForward);
        transform.rotation.SetLookRotation(player.transform.position - transform.position);
    }

    private void FireProjectile() {
        Projectile newProjectile = Instantiate(projectile, projectileSocket.transform.position, Quaternion.identity);

        Vector3 unitVectorToPlayer = (player.transform.position - projectileSocket.transform.position).normalized;
        float projectileSpeed = newProjectile.Speed;
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
