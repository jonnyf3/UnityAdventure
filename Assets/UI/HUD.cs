using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Combat;
using RPG.Actions;
using RPG.Quests;

namespace RPG.UI
{
    public class HUD : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] Slider healthBar = null;

        [Header("Weapons")]
        [SerializeField] Image weaponIcon = null;

        [Header("Special Abilities")]
        [SerializeField] GameObject abilityDisplay = null;
        [SerializeField] Image abilityIcon  = null;
        [SerializeField] Image energyAvailableMeter = null;
        private SpecialAbilities abilities;

        [Header("Treasure Counter")]
        [SerializeField] GameObject treasureDisplay = null;

        [Header("Objectives")]
        [SerializeField] GameObject questDisplay = null;
        [SerializeField] Sprite objectiveMarker = null;
        [SerializeField] Color markerColor = default;
        private Text questText;
        private Text objectiveText;

        [Header("Interaction")]
        [SerializeField] Sprite interactionIcon = null;

        //TODO make tutorial UI a separate canvas? (additive scene?)
        [Header("Tutorials")]
        [SerializeField] GameObject tutorialUI = null;

        Player player;

        void Awake() {
            player = FindObjectOfType<Player>();
            Assert.IsNotNull(player, "Could not find player in the scene!");

            //The inital delegate events may be missed if this waited until Start
            player.GetComponent<Health>().onHealthChanged += UpdateHealthBar;
            player.GetComponent<WeaponSystem>().onChangedWeapon += OnChangedWeapon;

            abilities = player.GetComponent<SpecialAbilities>();
            abilities.onChangedAbility += OnChangedAbility;

            questText = questDisplay.transform.GetChild(0).GetComponent<Text>();
            objectiveText = questDisplay.transform.GetChild(1).GetComponent<Text>();
            player.GetComponent<Journal>().onQuestChanged += UpdateQuestDisplay;
        }

        private void Start() {
            if (!abilities.HasAbilities) { abilityDisplay.SetActive(false); }

            tutorialUI.SetActive(false);
        }

        private void Update() {
            energyAvailableMeter.fillAmount = 1f - abilities.CooldownPercent;
        }

        #region Health
        private Coroutine healthCoroutine;
        void UpdateHealthBar(float percent) {
            if (healthCoroutine != null) { StopCoroutine(healthCoroutine); }

            ShowUI(healthBar.gameObject);
            healthBar.value = percent;

            healthCoroutine = StartCoroutine(FadeUI(healthBar.gameObject));
        }
        #endregion

        #region Equipment
        void OnChangedWeapon(Weapon newWeapon) {
            weaponIcon.sprite = newWeapon.sprite;
        }
        void OnChangedAbility(Sprite newAbility) {
            abilityDisplay.SetActive(true);
            abilityIcon.sprite = newAbility;
        }
        #endregion

        #region Treasure
        private Coroutine treasureCoroutine;
        public void UpdateTreasureText(int treasureCount, Color color) {
            if (treasureCoroutine != null) { StopCoroutine(treasureCoroutine); }

            ShowUI(treasureDisplay);
            treasureDisplay.GetComponentInChildren<Text>().text = treasureCount.ToString();
            treasureDisplay.GetComponentInChildren<Image>().color = color;

            treasureCoroutine = StartCoroutine(FadeUI(treasureDisplay));
        }
        #endregion

        #region Quests
        private Coroutine questCoroutine;
        private void UpdateQuestDisplay(string quest, List<string> objectives) {
            if (questCoroutine != null) { StopCoroutine(questCoroutine); }
            ShowUI(questDisplay);

            questText.text = quest;
            string s = "";
            foreach (var o in objectives) { s += " - " + o + "\n"; }
            objectiveText.text = s;

            questCoroutine = StartCoroutine(FadeUI(questDisplay));
        }
        #endregion

        #region Tutorials
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
        #endregion

        #region On-Screen Icons
        private List<GameObject> onScreenMarkers = new List<GameObject>();
        private void AddUIMarker(GameObject marker, Sprite sprite, Vector2 size) {
            marker.transform.SetParent(transform, true);
            marker.GetComponent<RectTransform>().sizeDelta = size;

            var image = marker.AddComponent<Image>();
            image.sprite = sprite;

            StartCoroutine(FadeUI(marker));
            onScreenMarkers.Add(marker);
        }
        public RectTransform AddObjectiveMarker() {
            var marker = new GameObject("Objective Marker", typeof(RectTransform));

            AddUIMarker(marker, objectiveMarker, new Vector2(15, 15));
            marker.GetComponent<Image>().color = markerColor;

            return marker.GetComponent<RectTransform>();
        }
        public RectTransform AddInteractionMarker() {
            var marker = new GameObject("Interaction Marker", typeof(RectTransform));
            AddUIMarker(marker, interactionIcon, new Vector2(30, 30));
            return marker.GetComponent<RectTransform>();
        }

        public void SetMarkerPosition(RectTransform marker, Vector3 position) {
            if (Vector3.Dot(Camera.main.transform.forward, position - player.transform.position) > 0) {
                marker.gameObject.SetActive(true);
                marker.position = Camera.main.WorldToScreenPoint(position);
            } else {
                marker.gameObject.SetActive(false);
            }
        }
        public void RemoveMarker(RectTransform marker) {
            if (!marker) { return; }
            onScreenMarkers.Remove(marker.gameObject);
            Destroy(marker.gameObject);
        }
        #endregion

        public void ShowAllUI() {
            StopAllCoroutines();

            ShowUI(healthBar.gameObject);
            StartCoroutine(FadeUI(healthBar.gameObject));

            ShowUI(treasureDisplay);
            StartCoroutine(FadeUI(treasureDisplay));

            ShowUI(questDisplay);
            StartCoroutine(FadeUI(questDisplay));
            foreach (var marker in onScreenMarkers) {
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
            if (!uiElement) { yield break; }
            foreach (var graphic in uiElement.GetComponentsInChildren<Graphic>()) {
                graphic.CrossFadeAlpha(0f, 2f, true);
            }
        }
    }
}