using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Magic;

namespace RPG.Characters
{
    public class SpecialCombat : MonoBehaviour
    {
        private Character character;

        //TODO make private, assign/extend via "unlocks"
        [SerializeField] List<MagicData> magicAbilities = new List<MagicData>(0);

        [Header("Energy Parameters")]
        [SerializeField] float maxEnergy = 10f;
        [SerializeField] float energyRegenPerSecond = 1f;
        [SerializeField] float energyRegenCooldown = 3f;

        [Header("UI")]
        [SerializeField] Slider energyBar = null;
        //[SerializeField] AudioClip outOfEnergy = null;

        private float currentEnergy;
        private float EnergyPercent { get { return currentEnergy / maxEnergy; } }
        private float lastEnergyUseTime;

        private MagicData currentMagic;
        private MagicData CurrentMagic {
            get { return currentMagic; }
            set {
                currentMagic = value;
                EquipMagicBehaviour();
            }
        }
        public delegate void OnChangedMagic(MagicData magic);
        public event OnChangedMagic onChangedMagic;
        
        private void Start() {
            character = GetComponent<Character>();

            currentEnergy = 0;
            lastEnergyUseTime = Time.time;
            energyBar.value = 0;

            if (magicAbilities.Count > 0) {
                CurrentMagic = magicAbilities[0];
                onChangedMagic(CurrentMagic);
            }
        }

        void Update() {
            if (Time.time - lastEnergyUseTime >= energyRegenCooldown) {
                RestoreEnergy(energyRegenPerSecond * Time.deltaTime);
            }
        }

        public void UseMagic() {
            if (!CurrentMagic) { return; }

            if (currentEnergy < currentMagic.EnergyCost) {
                print("Insufficient energy!");
                //GetComponent<AudioSource>().PlayOneShot(outOfEnergy);
                return;
            }
            
            CurrentMagic.Use();
        }

        public void AbilityUsed() {
            //Called by current magic Behaviour at the point when the ability is executed
            character.DoCustomAnimation(CurrentMagic.AnimClip);
            lastEnergyUseTime = Time.time;
            UseEnergy(CurrentMagic.EnergyCost);
        }
        
        public void CycleMagic(int step) {
            if (magicAbilities.Count == 0) { return; }

            var currentIndex = magicAbilities.IndexOf(CurrentMagic);
            var newIndex = (int)Mathf.Repeat(currentIndex + step, magicAbilities.Count);
            CurrentMagic = magicAbilities[newIndex];
        }

        public void UnlockAbility(MagicData newMagic) {
            magicAbilities.Add(newMagic);
            CurrentMagic = newMagic;
        }

        private void EquipMagicBehaviour() {
            var currentBehaviour = GetComponent<MagicBehaviour>();
            if (currentBehaviour != null) { Destroy((currentBehaviour as Component)); }

            CurrentMagic.AttachBehaviourTo(gameObject);

            onChangedMagic?.Invoke(CurrentMagic);
        }
        
        private void UseEnergy(float amount) {
            currentEnergy = Mathf.Clamp(currentEnergy - amount, 0, maxEnergy);
            energyBar.value = EnergyPercent;
        }
        private void RestoreEnergy(float amount) {
            UseEnergy(-amount);
        }
    }
}