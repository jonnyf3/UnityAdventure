using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Characters;
using RPG.Control;
using RPG.UI;
using RPG.Saving;
using Cinemachine;

namespace RPG.Actions
{
    public class AbilityMonolith : Interactable, ISaveable
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

            yield return new WaitForEndOfFrame();
            player.GetComponent<SpecialAbilities>().UnlockAbility(abilityToUnlock);
            hud.ShowTutorial(abilityTutorial);

            if (icon) { hud.RemoveMarker(icon); }
            enabled = false;
        }

        private PlayableAsset PlayCutscene() {
            player.StopControl();
            player.transform.forward = Vector3.ProjectOnPlane(transform.position - player.transform.position, Vector3.up);

            hud.gameObject.SetActive(false);

            var particles = GetComponentInChildren<ParticleSystem>();
            particles.transform.forward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
            particles.Play();

            var cinematic = GetComponent<PlayableDirector>();
            SetupCinemachineTrack(cinematic);
            cinematic.Play();
            return cinematic.playableAsset;
        }
        private void SetupCinemachineTrack(PlayableDirector cinematic) {
            var timelineTracks = new List<PlayableBinding>(cinematic.playableAsset.outputs);
            var cinemachineTrack = timelineTracks[0].sourceObject;
            cinematic.SetGenericBinding(cinemachineTrack, FindObjectOfType<CinemachineBrain>());
        }
        private void EndCutscene() {
            hud.gameObject.SetActive(true);
            hud.ShowAllUI();
            player.SetDefaultState();
        }

        #region SaveLoad
        public object SaveState() {
            return enabled;
        }
        public void LoadState(object state) {
            enabled = (bool)state;
        }
        #endregion
    }
}