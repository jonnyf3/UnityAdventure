using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;
using RPG.Actions;
using System;

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
        private SpecialAbilities abilities;

        [Header("Treasure Counter")]
        [SerializeField] GameObject treasureDisplay = null;
        private Coroutine treasureCoroutine;

        [Header("Objectives")]
        [SerializeField] Sprite objectiveMarker = null;
        [SerializeField] Color markerColor = default;

        //TODO make tutorial UI a separate canvas? (additive scene?)
        [Header("Tutorials")]
        [SerializeField] GameObject tutorialUI = null;

        Player player;
        
        //The inital delegate events may be missed if this waited until Start
        void Awake() {
            player = FindObjectOfType<Player>();
            Assert.IsNotNull(player, "Could not find player in the scene!");

            player.GetComponent<Health>().onHealthChanged += UpdateHealthBar;
            player.GetComponent<WeaponSystem>().onChangedWeapon += OnChangedWeapon;

            abilities = player.GetComponent<SpecialAbilities>();
            abilities.onChangedAbility += OnChangedAbility;
        }

        private void Start() {
            if (!abilities.HasAbilities) { abilityDisplay.SetActive(false); }

            tutorialUI.SetActive(false);
        }

        private void Update() {
            energyAvailableMeter.fillAmount = 1f - abilities.CooldownPercent;
        }

        //Health
        void UpdateHealthBar(float percent) {
            if (healthCoroutine != null) { StopCoroutine(healthCoroutine); }

            ShowUI(healthBar.gameObject);
            healthBar.value = percent;

            healthCoroutine = StartCoroutine(FadeUI(healthBar.gameObject));
        }

        //Equipment
        void OnChangedWeapon(Weapon newWeapon) {
            weaponIcon.sprite = newWeapon.sprite;
        }
        void OnChangedAbility(Sprite newAbility) {
            abilityDisplay.SetActive(true);
            abilityIcon.sprite = newAbility;
        }

        //Treasure
        public void UpdateTreasureText(int treasureCount, Color color) {
            if (treasureCoroutine != null) { StopCoroutine(treasureCoroutine); }

            ShowUI(treasureDisplay);
            treasureDisplay.GetComponentInChildren<Text>().text = treasureCount.ToString();
            treasureDisplay.GetComponentInChildren<Image>().color = color;

            treasureCoroutine = StartCoroutine(FadeUI(treasureDisplay));
        }

        //Tutorials
        private const string TUTORIAL_DISMISS_BTN = "X";

        public void ShowTutorial(Tutorial tutorial) {
            tutorialUI.SetActive(true);

            var header = tutorialUI.transform.GetChild(0);
            var body = tutorialUI.transform.GetChild(1);

            header.GetComponent<Text>().text = tutorial.title;
            body.GetComponent<Text>().text = tutorial.description;
            body.GetComponent<Text>().resizeTextForBestFit = true;

            StartCoroutine(CloseTutorial());
        }
        private IEnumerator CloseTutorial() {
            //pause game and prevent further player input
            Time.timeScale = 0f;
            player.StopControl();

            yield return new WaitUntil(() => Input.GetButtonDown(TUTORIAL_DISMISS_BTN));

            //resume game and control
            Time.timeScale = 1f;
            player.SetDefaultState();
            tutorialUI.SetActive(false);
        }

        //Objectives
        private GameObject marker;
        public RectTransform ShowObjectiveMarker(Vector3 position) {
            marker = new GameObject("Objective Marker", typeof(RectTransform));
            marker.transform.SetParent(transform, true);
            marker.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 15);

            var image = marker.AddComponent<Image>();
            image.sprite = objectiveMarker;
            image.color = markerColor;

            StartCoroutine(FadeUI(marker));
            return marker.GetComponent<RectTransform>();
        }
        public void SetMarkerPosition(RectTransform marker, Vector3 position) {
            marker.position = Camera.main.WorldToScreenPoint(position);
        }


        public void ShowAllUI() {
            StopAllCoroutines();

            ShowUI(healthBar.gameObject);
            StartCoroutine(FadeUI(healthBar.gameObject));

            ShowUI(treasureDisplay);
            StartCoroutine(FadeUI(treasureDisplay));

            if (marker) {
                ShowUI(marker);
                StartCoroutine(FadeUI(marker));
            }
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