﻿using RPG.Characters;

namespace RPG.States
{
    public class IdleState : State
    {
        private void Start() {
            if ((character as AICharacter) && (character as AICharacter).PatrolPath) {
                character.SetState<PatrollingState>();
            }
        }

        private void Update() {
            character.Move(transform.position);
        }
    }
}