using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Combat;

namespace RPG.Quests
{
    public class KillObjectiveBehaviour : ObjectiveBehaviour
    {
        private List<Health> targets;
        
        public override void Setup(Objective objectiveData) {
            var data = objectiveData as KillObjective;
            Assert.IsNotNull(data, "Wrong objective data type passed in");

            targets = new List<Health>();
            foreach (var id in data.Targets) {
                targets.Add(GameObject.Find(id).GetComponent<Health>());
            }

            foreach (var target in targets) {
                target.onDeath += () => KilledTarget(target);
            }
        }

        private void KilledTarget(Health target) {
            targets.Remove(target);
            if (targets.Count == 0) { CompleteObjective(); }
        }
    }
}