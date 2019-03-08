using UnityEngine;
using RPG.States;

namespace RPG.Characters
{
    public class Player : Character
    {
        public Transform projectileSpawn { get; private set; }
        public Transform abilitySpawn    { get; private set; }

        protected override void Start() {
            base.Start();

            allyState = AllyState.Ally;

            SetControlled();
        }

        public void SetControlled() {
            SetState<ControlledState>();
        }
        public void StopControl() {
            SetState<IdleState>();
        }
        
        public void SetRangedSpawnPoint(Transform spawnPoint) {
            projectileSpawn = spawnPoint;
        }
        public void SetAbilitySpawnPoint(Transform spawnPoint)  {
            abilitySpawn = spawnPoint;
        }
    }
}