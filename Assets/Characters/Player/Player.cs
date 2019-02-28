using UnityEngine;
using RPG.States;
using RPG.Control;
using RPG.Combat;
using RPG.Actions;

namespace RPG.Characters
{
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(SpecialAbilities))]
    public class Player : Character
    {
        private Transform projectileSpawn;
        private Transform abilitySpawn;

        protected override void Start() {
            base.Start();

            allyState = AllyState.Ally;

            var controlArgs = new ControlledStateArgs(this, projectileSpawn, abilitySpawn);
            SetState<ControlledState>(controlArgs);
        }
        
        public void SetRangedSpawnPoint(Transform spawnPoint) {
            projectileSpawn = spawnPoint;
            var controlArgs = new ControlledStateArgs(this, projectileSpawn, abilitySpawn);
            SetState<ControlledState>(controlArgs);
        }
        public void SetAbilitySpawnPoint(Transform spawnPoint)  {
            abilitySpawn = spawnPoint;
            var controlArgs = new ControlledStateArgs(this, projectileSpawn, abilitySpawn);
            SetState<ControlledState>(controlArgs);
        }
    }
}