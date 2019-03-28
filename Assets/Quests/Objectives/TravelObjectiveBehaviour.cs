using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Quests
{
    public class TravelObjectiveBehaviour : ObjectiveBehaviour
    {
        private Transform destination;
        private float requiredProximity;

        private Transform player;
        private float DistanceToDestination => Vector3.Distance(player.position, destination.position);

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
        }

        private void Update() {
            ShowObjectiveMarker(destination.position);

            if (DistanceToDestination <= requiredProximity) {
                RemoveHUDMarker();
                CompleteObjective();
            }
        }
    }
}