using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] float maxEnergy = 10f;
        [SerializeField] float energyRegenPerSecond = 1f;
        [SerializeField] float energyRegenCooldown = 3f;
        [SerializeField] Slider energyBar = null;
        private float currentEnergy;
        private float lastEnergyUseTime;

        public float EnergyPercent {
            get { return currentEnergy / maxEnergy; }
        }

        // Start is called before the first frame update
        void Start() {
            currentEnergy = 0;
            lastEnergyUseTime = Time.time;

            Assert.IsNotNull(energyBar, "Could not find an energy bar UI element, is it assigned?");
            energyBar.value = 0;
        }

        void Update() {
            if (Time.time - lastEnergyUseTime >= energyRegenCooldown) {
                RestoreEnergy(energyRegenPerSecond * Time.deltaTime);
            }
        }

        public bool hasEnoughEnergy(float energyCost) {
            return currentEnergy >= energyCost;
        }
        public void UseEnergy(float amount) {
            currentEnergy = Mathf.Clamp(currentEnergy - amount, 0, maxEnergy);
            energyBar.value = EnergyPercent;

            //don't set lastEnergyUseTime if energy is being restored
            if (amount > 0) { lastEnergyUseTime = Time.time; }
        }
        private void RestoreEnergy(float amount) {
            UseEnergy(-amount);
        }
    }
}