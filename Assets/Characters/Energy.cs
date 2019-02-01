﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] float maxEnergy = 10f;
        [SerializeField] float energyRegenPerSecond = 1f;
        [SerializeField] Slider energyBar = null;
        private float currentEnergy;

        public float EnergyPercent {
            get { return currentEnergy / maxEnergy; }
        }

        // Start is called before the first frame update
        void Start() {
            currentEnergy = 0;

            Assert.IsNotNull(energyBar, "Could not find an energy bar UI element, is it assigned?");
            energyBar.value = 0;
        }

        void Update() {
            RestoreEnergy(energyRegenPerSecond * Time.deltaTime);
        }

        private void RestoreEnergy(float amount) {
            currentEnergy += amount;
            Mathf.Clamp(currentEnergy, 0, maxEnergy);

            energyBar.value = EnergyPercent;
        }
    }
}