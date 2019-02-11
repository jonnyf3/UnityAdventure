using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Magic
{
    public class SpecialCombat : MonoBehaviour
    {
        //TODO make private, assign/extend via "unlocks"
        [SerializeField] List<MagicData> magicAbilities = new List<MagicData>(0);

        [Header("Energy Parameters")]
        [SerializeField] float maxEnergy = 10f;
        [SerializeField] float energyRegenPerSecond = 1f;
        [SerializeField] float energyRegenCooldown = 3f;

        [Header("UI")]
        [SerializeField] Slider energyBar;

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
                return;
            }

            //TODO restore specific animation
            //SetAttackAnimation(currentMagic.AnimClip);
            UseEnergy(CurrentMagic.EnergyCost);
            lastEnergyUseTime = Time.time;
            CurrentMagic.Use();
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

            onChangedMagic(CurrentMagic);
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