using UnityEngine;
using RPG.UI;
using UnityEngine.SceneManagement;

namespace RPG.Quests
{
    public abstract class ObjectiveBehaviour : MonoBehaviour
    {
        private Objective objective;
        public virtual void Setup(Objective objectiveData) {
            objective = objectiveData;

            SceneManager.activeSceneChanged += (s1, s2) => Destroy(this);
        }

        protected void CompleteObjective() {
            print("Objective complete!");
            objective.Complete();
            Destroy(this);
        }

        //HUD
        private HUD hud;
        private RectTransform marker;

        protected void ShowObjectiveMarker(Vector3 position) {
            if (!hud) {
                hud = FindObjectOfType<HUD>();
                if (!hud) { return; }
            }
            if (!marker) { marker = hud.AddObjectiveMarker(); }

            hud.SetMarkerPosition(marker, position);
        }
        protected void RemoveHUDMarker() {
            if (!marker) { return; }
            hud.RemoveMarker(marker);
        }
    }
}