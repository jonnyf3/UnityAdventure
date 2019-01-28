using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] int value = 5;
    [SerializeField] GameObject collectionFX = null;
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

    private void OnCollisionEnter(Collision collision) {
        var player = collision.gameObject.transform.parent;
        var treasureCollector = player.GetComponentInChildren<TreasureCollector>();
        if (treasureCollector) {
            treasureCollector.Collect(value);

            //TODO remove particles gameobject from scene after firing
            Instantiate(collectionFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
