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

        [Header("UI")]
        [SerializeField] Image energyAvailableMeter = null;
        //[SerializeField] AudioClip outOfEnergy = null;

        [Header("Energy Parameters")]
        [SerializeField] float energyRegenPerSecond = 1f;
        //[SerializeField] float energyRegenCooldown = 3f;

        private float timeSinceEnergyUse;

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

            timeSinceEnergyUse = 0f;
            energyAvailableMeter.fillAmount = 1f;

            if (magicAbilities.Count > 0) {
                CurrentMagic = magicAbilities[0];
                onChangedMagic(CurrentMagic);
            }
        }

        void Update() {
            timeSinceEnergyUse += energyRegenPerSecond * Time.deltaTime;
            var cooldownPercent = Mathf.Clamp(timeSinceEnergyUse / CurrentMagic.CooldownTime, 0f, 1f);
            energyAvailableMeter.fillAmount = 1 - cooldownPercent;
        }

        public void UseMagic() {
            if (!CurrentMagic) { return; }

            if (timeSinceEnergyUse >= CurrentMagic.CooldownTime) { CurrentMagic.Use(); }
        }

        public void AbilityUsed() {
            //Called by current magic Behaviour at the point when the ability is executed
            character.DoCustomAnimation(CurrentMagic.AnimClip);
            timeSinceEnergyUse = 0f;
            energyAvailableMeter.fillAmount = 1f;
        }
        
        public void CycleMagic(int step) {
            if (magicAbilities.Count < 2) { return; }

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
    }
}