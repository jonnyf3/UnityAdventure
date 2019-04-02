using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Characters;

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

        //TODO move reload code to an object which is never destroyed, subscribe to player onDeath event
        private IEnumerator ReloadLevel() {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}