using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.UI;

namespace RPG.Actions
{
    public class SpecialAbilities : MonoBehaviour
    {
        private Character character;
        private HUD hud;    //TODO having HUD as member assumes only player will ever have special abilities?

        //TODO make private, assign/extend via "unlocks"
        [SerializeField] List<AbilityData> abilities = new List<AbilityData>(0);

        [Header("Energy Parameters")]
        [SerializeField] float energyRegenPerSecond = 1f;
        //[SerializeField] float energyRegenCooldown = 3f;
        //[SerializeField] AudioClip outOfEnergy = null;

        private const string ANIMATOR_ATTACK_PARAM = "onAttack";
        private float timeSinceEnergyUse;

        private AbilityData currentAbility;
        private AbilityData CurrentAbility {
            get { return currentAbility; }
            set {
                currentAbility = value;
                EquipAbility();
            }
        }
        public delegate void OnChangedAbility(Sprite sprite);
        public event OnChangedAbility onChangedAbility;

        private void Start() {
            character = GetComponent<Character>();
            hud = FindObjectOfType<HUD>();

            timeSinceEnergyUse = 0f;

            if (abilities.Count > 0) {
                CurrentAbility = abilities[0];
                hud.UpdateAbilityAvailability(0);
            } else {
                hud.ShowAbilityIcon(false);
            }
        }

        void Update() {
            if (abilities.Count == 0) { return; }

            timeSinceEnergyUse += energyRegenPerSecond * Time.deltaTime;
            var cooldownPercent = Mathf.Clamp(timeSinceEnergyUse / CurrentAbility.CooldownTime, 0f, 1f);

            hud.UpdateAbilityAvailability(cooldownPercent);
        }

        public void Use() {
            if (!CurrentAbility) { return; }

            if (timeSinceEnergyUse >= CurrentAbility.CooldownTime) { CurrentAbility.Use(); }
        }

        public void AbilityUsed() {
            //Called by current AbilityBehaviour at the point when the ability is executed
            DoAbilityAnimation();
            timeSinceEnergyUse = 0f;
            hud.UpdateAbilityAvailability(0f);
        }

        private void DoAbilityAnimation() {
            var animator = GetComponent<Animator>();
            var animOverride = animator.runtimeAnimatorController as AnimatorOverrideController;
            Assert.IsNotNull(animOverride, gameObject + " has no animator override controller to set custom animation!");
            animOverride["DEFAULT ATTACK"] = CurrentAbility.AnimClip;

            animator.SetTrigger(ANIMATOR_ATTACK_PARAM);
        }

        public void CycleAbilities(int step) {
            if (abilities.Count < 2) { return; }

            var currentIndex = abilities.IndexOf(CurrentAbility);
            var newIndex = (int)Mathf.Repeat(currentIndex + step, abilities.Count);
            CurrentAbility = abilities[newIndex];
        }

        public void UnlockAbility(AbilityData ability) {
            hud.ShowAbilityIcon(true);
            abilities.Add(ability);
            CurrentAbility = ability;
        }

        private void EquipAbility() {
            var currentBehaviour = GetComponent<AbilityBehaviour>();
            if (currentBehaviour != null) { Destroy((currentBehaviour as Component)); }

            CurrentAbility.AttachBehaviourTo(gameObject);

            onChangedAbility?.Invoke(CurrentAbility.Sprite);
        }
    }
}