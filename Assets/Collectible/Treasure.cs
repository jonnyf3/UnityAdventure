using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] int value = 5;
    private bool collected = false;
    private Transform destination;
    private float speed;

    public void Attract(Transform destination, float speed) {
        this.destination = destination;
        this.speed = speed;
        collected = true;
    }

    private void Update() {
        if (collected) {
            Vector3 unitVectorToPlayer = (destination.position - transform.position).normalized;
            Vector3 gatherVelocity = unitVectorToPlayer * speed;
            GetComponent<Rigidbody>().velocity = gatherVelocity;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        var collider = collision.collider.gameObject;
        var treasureCollector = collider.transform.parent.GetComponentInChildren<TreasureCollector>();
        if (treasureCollector) {
            treasureCollector.Collect(value);
            Destroy(gameObject);
        }
    }
}
