using UnityEngine;
using UnityEngine.Assertions;
using RPG.UI;

namespace RPG.Quests
{
    public class TravelObjectiveBehaviour : ObjectiveBehaviour
    {
        private Transform destination;
        private float requiredProximity;

        private Transform player;
        private float DistanceToDestination => Vector3.Distance(player.position, destination.position);

        private HUD hud;
        private RectTransform marker;

        public override void Setup(Objective objectiveData) {
            base.Setup(objectiveData);

            var data = objectiveData as TravelObjective;
            Assert.IsNotNull(data, "Wrong objective data type passed in");
            var destinationObject = GameObject.Find(data.Destination);
            if (destinationObject) { destination = destinationObject.GetComponent<Transform>(); }
            requiredProximity = data.RequiredProximity;
        }

        private void Start()  {
            player = FindObjectOfType<Journal>().transform;
            Assert.IsNotNull(player, "Could not find player in scene - does the player have a Journal component?");

            hud = FindObjectOfType<HUD>();
            marker = hud.ShowObjectiveMarker(destination.position);
        }

        private void Update() {
            hud.SetMarkerPosition(marker, destination.position);

            if (DistanceToDestination <= requiredProximity) {
                Destroy(marker.gameObject);
                CompleteObjective();
            }
        }
    }
}