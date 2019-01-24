using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TreasureCollector : MonoBehaviour
{
    [SerializeField] float range = 1.5f;
    [SerializeField] float gatherSpeed = 8f;
    [SerializeField] Transform collectionTarget = null;

    private SphereCollider collector = null; 
    
    // Start is called before the first frame update
    void Start() {
        collector = GetComponent<SphereCollider>();
        collector.radius = range;
    }

    private void OnTriggerEnter(Collider other) {
        var treasure = other.GetComponent<Treasure>();
        if (treasure) {
            treasure.Attract(collectionTarget, gatherSpeed);
        }
    }

    public void Collect(int value) {
        print("Picked up treasure with value " + value);
    }
}
