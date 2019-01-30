using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerMovement : MonoBehaviour
{
    private ThirdPersonCharacter character = null;
    
    // Start is called before the first frame update
    void Start() {
        character = GetComponentInChildren<ThirdPersonCharacter>();
    }

    public void Move(Vector3 movement) {
        //pass Move command to ThirdPersonCharacter script
        character.Move(movement, false, false);
    }

    private void LateUpdate() {
        //Move command causes the body to move relative to the main Player object
        //Ensure camera follows player and reset local positon to zero
        transform.position += character.transform.localPosition; ;
        character.transform.localPosition = Vector3.zero;
    }
}