using UnityEngine;
using UnityEngine.Assertions;
using RPG.Actions;

namespace RPG.Quests
{
    public class InteractObjectiveBehaviour : ObjectiveBehaviour
    {
        private Interactable target;
        private Transform player;
        private float PlayerProximity => Vector3.Distance(player.position, target.transform.position);

        public override void Setup(Objective objectiveData) {
            base.Setup(objectiveData);

            var data = objectiveData as InteractObjective;
            Assert.IsNotNull(data, "Wrong objective data type passed in");

            var interactable = GameObject.Find(data.Target);
            if (interactable) {
                target = interactable.GetComponent<Interactable>();
                target.onInteraction += CompleteObjective;
            }
        }

        private void Start() {
            player = FindObjectOfType<Journal>().transform;
            Assert.IsNotNull(player, "Could not find player in scene - does the player have a Journal component?");
        }
        
        private void Update() {
            ShowObjectiveMarker(target.transform.position);
            if (PlayerProximity < 5f) {
                RemoveHUDMarker();
            }
        }
    }
}