using System.Collections;
using UnityEngine;
using RPG.Characters;
using RPG.SceneManagement;

namespace RPG.States
{
    public class DeadState : State
    {
        private const string ANIMATOR_DEATH_PARAM = "onDeath";

        private void Start() {
            character.GetComponent<Animator>().SetTrigger(ANIMATOR_DEATH_PARAM);
            
            if (character as Player) {
                PlayerDied(character as Player);
            }
            else if (character as Enemy) {
                EnemyDied(character as Enemy);
            }
            else if (character as NPC) {
                NPCDied(character as NPC);
            }
            else {
                print("Unknown character type");
            }
        }

        void PlayerDied(Player character) {
            StartCoroutine(ReloadLevel());
        }
        void EnemyDied(Enemy character) {
            character.Target = null;
            character.Move(transform.position);
            GetComponent<CapsuleCollider>().enabled = false;
            Destroy(gameObject, 3f);
        }
        void NPCDied(NPC character) {
            character.Move(transform.position);
            character.Respawn();
        }

        private IEnumerator ReloadLevel() {
            yield return new WaitForSeconds(5f);
            var sc = FindObjectOfType<SceneController>();
            sc.ReloadLevel();
        }
    }
}