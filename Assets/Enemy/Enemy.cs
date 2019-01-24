using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
    [SerializeField] float attackRadius = 5f;

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
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius) {
            ai.SetTarget(player.transform);
        }
        else {
            ai.SetTarget(transform);
        }
    }

    private void LateUpdate() {
        transform.position += ai.transform.localPosition; ;
        ai.transform.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
