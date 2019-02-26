using System.Collections.Generic;
using UnityEngine;
using RPG.Magic;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class SpecialCombat : MonoBehaviour
    {
        private Character character;
        private HUD hud;    //TODO having HUD as member assumes only player will ever have special abilities?

        //TODO make private, assign/extend via "unlocks"
        [SerializeField] List<MagicData> magicAbilities = new List<MagicData>(0);

        [Header("Energy Parameters")]
        [SerializeField] float energyRegenPerSecond = 1f;
        //[SerializeField] float energyRegenCooldown = 3f;
        //[SerializeField] AudioClip outOfEnergy = null;

        private float timeSinceEnergyUse;

        private MagicData currentMagic;
        private MagicData CurrentMagic {
            get { return currentMagic; }
            set {
                currentMagic = value;
                EquipMagicBehaviour();
            }
        }
        public delegate void OnChangedMagic(Sprite magicSprite);
        public event OnChangedMagic onChangedMagic;

        private void Start() {
            character = GetComponent<Character>();
            hud = FindObjectOfType<HUD>();

            timeSinceEnergyUse = 0f;

            if (magicAbilities.Count > 0) {
                CurrentMagic = magicAbilities[0];
                hud.UpdateAbilityAvailability(0);
            } else {
                hud.ShowAbilityIcon(false);
            }
        }

        void Update() {
            if (magicAbilities.Count == 0) { return; }

            timeSinceEnergyUse += energyRegenPerSecond * Time.deltaTime;
            var cooldownPercent = Mathf.Clamp(timeSinceEnergyUse / CurrentMagic.CooldownTime, 0f, 1f);

            hud.UpdateAbilityAvailability(cooldownPercent);
        }

        public void UseMagic() {
            if (!CurrentMagic) { return; }

            if (timeSinceEnergyUse >= CurrentMagic.CooldownTime) { CurrentMagic.Use(); }
        }

        public void AbilityUsed() {
            //Called by current magic Behaviour at the point when the ability is executed
            character.DoCustomAnimation(CurrentMagic.AnimClip);
            timeSinceEnergyUse = 0f;
            hud.UpdateAbilityAvailability(0f);
        }
        
        public void CycleMagic(int step) {
            if (magicAbilities.Count < 2) { return; }

            var currentIndex = magicAbilities.IndexOf(CurrentMagic);
            var newIndex = (int)Mathf.Repeat(currentIndex + step, magicAbilities.Count);
            CurrentMagic = magicAbilities[newIndex];
        }

        public void UnlockAbility(MagicData newMagic) {
            hud.ShowAbilityIcon(true);
            magicAbilities.Add(newMagic);
            CurrentMagic = newMagic;
        }

        private void EquipMagicBehaviour() {
            var currentBehaviour = GetComponent<MagicBehaviour>();
            if (currentBehaviour != null) { Destroy((currentBehaviour as Component)); }

            CurrentMagic.AttachBehaviourTo(gameObject);

            onChangedMagic?.Invoke(CurrentMagic.Sprite);
        }
    }
}