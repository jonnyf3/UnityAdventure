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
            character.StopMoving();
            Destroy(gameObject, 3f);
        }
        void NPCDied(NPC character) {
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