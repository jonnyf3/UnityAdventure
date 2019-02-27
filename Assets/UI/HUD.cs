using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;
using RPG.Actions;

namespace RPG.UI
{
    public class HUD : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] Slider healthBar = null;
        private Coroutine healthCoroutine;

        [Header("Weapons")]
        [SerializeField] Image weaponIcon = null;

        [Header("Special Abilities")]
        [SerializeField] GameObject abilityDisplay = null;
        [SerializeField] Image abilityIcon  = null;
        [SerializeField] Image energyAvailableMeter = null;

        [Header("Treasure Counter")]
        [SerializeField] GameObject treasureDisplay = null;
        private Coroutine treasureCoroutine;
        
        //The inital delegate events may be missed if this waited until Start
        void Awake() {
            var player = FindObjectOfType<PlayerController>();
            Assert.IsNotNull(player, "Could not find player in the scene!");

            player.GetComponent<Health>().onHealthChanged += UpdateHealthBar;
            player.GetComponent<WeaponSystem>().onChangedWeapon += OnChangedWeapon;
            player.GetComponent<SpecialAbilities>().onChangedAbility += OnChangedAbility;
        }

        //Health
        void UpdateHealthBar(float percent) {
            if (healthCoroutine != null) { StopCoroutine(healthCoroutine); }

            ShowUI(healthBar.gameObject);
            healthBar.value = percent;

            healthCoroutine = StartCoroutine(FadeUI(healthBar.gameObject));
        }

        //Abilities
        public void UpdateAbilityAvailability(float cooldownPercent) {
            energyAvailableMeter.fillAmount = 1f - cooldownPercent;
        }
        public void ShowAbilityIcon(bool visible) { abilityDisplay.SetActive(visible); }

        //Equipment
        void OnChangedAbility(Sprite newAbility) { abilityIcon.sprite = newAbility; }
        void OnChangedWeapon(Sprite newWeapon) { weaponIcon.sprite = newWeapon; }

        //Treasure
        public void UpdateTreasureText(int treasureCount, Color color) {
            if (treasureCoroutine != null) { StopCoroutine(treasureCoroutine); }

            ShowUI(treasureDisplay);
            treasureDisplay.GetComponentInChildren<Text>().text = treasureCount.ToString();
            treasureDisplay.GetComponentInChildren<Image>().color = color;

            treasureCoroutine = StartCoroutine(FadeUI(treasureDisplay));
        }


        private void ShowUI(GameObject uiElement) {
            //Immediately set opacity to full
            foreach (var graphic in uiElement.GetComponentsInChildren<Graphic>()) {
                graphic.CrossFadeAlpha(1f, 0f, true);
            }
        }
        private IEnumerator FadeUI(GameObject uiElement) {
            //After a delay, fade to transparent
            yield return new WaitForSeconds(3f);
            foreach (var graphic in uiElement.GetComponentsInChildren<Graphic>()) {
                graphic.CrossFadeAlpha(0f, 2f, true);
            }
        }
    }
}