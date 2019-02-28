using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Characters;
using RPG.Combat;

namespace RPG.States
{
    public class DeadState : State
    {
        private const string ANIMATOR_DEATH_PARAM = "onDeath";

        public override void OnStateEnter(StateArgs args) {
            base.OnStateEnter(args);

            character.GetComponent<Animator>().SetTrigger(ANIMATOR_DEATH_PARAM);
            
            if (character as PlayerController) {
                PlayerDied(character as PlayerController);
            }
            else if (character as EnemyController) {
                EnemyDied(character as EnemyController);
            }
            else if (character as NPCController) {
                NPCDied(character as NPCController);
            }
            else {
                print("Unknown character type");
            }
        }

        void PlayerDied(PlayerController character) {
            StartCoroutine(ReloadLevel());
        }
        void EnemyDied(EnemyController character) {
            character.Target = null;
            character.StopMoving();
            Destroy(gameObject, 3f);
        }
        void NPCDied(NPCController character) {
            character.StopMoving();
            character.GetComponent<Health>().Respawn();
        }

        //TODO move reload code to an object which is never destroyed, subscribe to player onDeath event
        private IEnumerator ReloadLevel() {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene(0);
        }
    }
}