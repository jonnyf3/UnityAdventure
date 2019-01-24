using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerMovement : MonoBehaviour
{
    private ThirdPersonCharacter character;
    
    // Start is called before the first frame update
    void Start() {
        character = GetComponent<ThirdPersonCharacter>();
    }

    public void Move(Vector3 movement) {
        print(movement);
        character.Move(movement, false, false);
    }
}