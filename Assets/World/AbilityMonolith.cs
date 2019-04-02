using System.Collections;
using UnityEngine;
using RPG.Characters;
using RPG.Control;
using RPG.UI;
using UnityEngine.Playables;
using RPG.Movement;

namespace RPG.Actions
{
    public class AbilityMonolith : Interactable
    {
        [SerializeField] AbilityData abilityToUnlock = null;
        [SerializeField] Tutorial abilityTutorial = null;

        private Player player;
        private HUD hud;
        private RectTransform icon;

        void Start() {
            onInteraction += () => StartCoroutine(UseMonolith());

            player = FindObjectOfType<Player>();
            hud = FindObjectOfType<HUD>();
        }

        void Update() {
            if (Vector3.Distance(transform.position, player.transform.position) <= 2.5f) {
                if (!icon) { icon = hud.AddInteractionMarker(); }
                hud.SetMarkerPosition(icon, transform.position);

                if (Input.GetButtonDown(ControllerInput.INTERACT_BUTTON)) { Interact(); }
            } else {
                hud.RemoveMarker(icon);
            }
        }

        private IEnumerator UseMonolith() {
            var cinematic = PlayCutscene();
            yield return new WaitForSeconds((float)cinematic.duration);
            EndCutscene();

            player.GetComponent<SpecialAbilities>().UnlockAbility(abilityToUnlock);
            hud.ShowTutorial(abilityTutorial);

            if (icon) { hud.RemoveMarker(icon); }
            Destroy(this);
        }

        private PlayableAsset PlayCutscene() {
            player.StopControl();
            player.transform.forward = Vector3.ProjectOnPlane(transform.position - player.transform.position, Vector3.up);

            hud.gameObject.SetActive(false);

            var particles = GetComponentInChildren<ParticleSystem>();
            particles.transform.forward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
            particles.Play();

            var cinematic = GetComponent<PlayableDirector>();
            cinematic.Play();
            return cinematic.playableAsset;
        }
        private void EndCutscene() {
            hud.gameObject.SetActive(true);
            hud.ShowAllUI();
            player.SetDefaultState();
        }
    }
}