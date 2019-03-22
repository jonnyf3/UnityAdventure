using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Quests
{
    public class TravelObjective : Objective
    {
        [Header("Destination")]
        [SerializeField] Transform destination = default;
        [SerializeField] float requiredProximity = 1f;

        private Transform player;
        private float DistanceToDestination => Vector3.Distance(player.position, destination.position);

        public override void Activate() {
            Assert.IsNotNull(destination, "This objective has no destination");

            player = FindObjectOfType<Journal>().transform;
            Assert.IsNotNull(player, "Could not find player in scene - does the player have a Journal component?");

            StartCoroutine(DestinationCheck());
        }

        private IEnumerator DestinationCheck() {
            yield return new WaitUntil(() => (DistanceToDestination <= requiredProximity));
            CompleteObjective();
        }

        private void OnDrawGizmosSelected() {
            if (destination) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(destination.position, requiredProximity);
            }
        }
    }
}