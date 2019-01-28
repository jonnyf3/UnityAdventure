using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupCamera : MonoBehaviour
{
    private Camera camera;
    private Health playerHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        if (camera.isActiveAndEnabled) { camera.enabled = false; }

        var player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<Health>();

        playerHealth.onDeath += MakeActiveCamera;
    }

    void MakeActiveCamera() {
        camera.enabled = true;
    }
}
