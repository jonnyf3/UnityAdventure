using UnityEngine;
using RPG.States;
using RPG.Saving;

namespace RPG.Characters
{
    public class Player : Character, ISaveable
    {
        public Transform projectileSpawn { get; private set; }
        public Transform abilitySpawn    { get; private set; }

        protected override void Start() {
            base.Start();
            allyState = AllyState.Ally;
        }

        public override void Move(Vector3 destination, float maxForwardCap = 1f) {
            movement.Move(destination - transform.position, maxForwardCap);
        }

        public override void SetDefaultState() { SetState<ControlledState>(); }
        public void StopControl() { SetState<IdleState>(); }
        
        public void SetRangedSpawnPoint(Transform spawnPoint) {
            projectileSpawn = spawnPoint;
        }
        public void SetAbilitySpawnPoint(Transform spawnPoint)  {
            abilitySpawn = spawnPoint;
        }

        #region SaveLoad
        public object SaveState() {
            return new CharacterSaveData(transform.position, transform.eulerAngles);
        }

        public void LoadState(object state) {
            var charState = (CharacterSaveData)state;
            transform.position = charState.position.ToVector();
            transform.eulerAngles = charState.rotation.ToVector();
        }
        #endregion
    }
}