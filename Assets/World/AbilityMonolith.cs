using System.Collections;
using UnityEngine;
using RPG.Characters;
using RPG.Control;
using RPG.UI;

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
            onInteraction += () => StartCoroutine(UnlockAbility());

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

        private IEnumerator UnlockAbility() {
            var particles = GetComponentInChildren<ParticleSystem>();
            particles.transform.forward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
            
            //TODO cinematic
            particles.Play();
            player.StopControl();
            hud.gameObject.SetActive(false);

            yield return new WaitForSeconds(particles.main.duration + particles.main.startLifetime.constant);
            
            hud.gameObject.SetActive(true);
            player.SetDefaultState();
            player.GetComponent<SpecialAbilities>().UnlockAbility(abilityToUnlock);
            hud.ShowTutorial(abilityTutorial);

            if (icon) { hud.RemoveMarker(icon); }
            Destroy(this);
        }
    }
}