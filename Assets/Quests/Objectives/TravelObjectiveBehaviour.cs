using System.Collections;
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
            destination = GameObject.Find(data.Destination).GetComponent<Transform>();
            requiredProximity = data.RequiredProximity;

            player = FindObjectOfType<Journal>().transform;
            Assert.IsNotNull(player, "Could not find player in scene - does the player have a Journal component?");

            StartCoroutine(DestinationCheck());
        }

        private IEnumerator DestinationCheck() {
            yield return new WaitUntil(() => (DistanceToDestination <= requiredProximity));
            CompleteObjective();
        }
    }
}