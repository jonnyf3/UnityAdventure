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
            base.Setup(objectiveData);

            var data = objectiveData as KillObjective;
            Assert.IsNotNull(data, "Wrong objective data type passed in");

            targets = new List<Health>();
            foreach (var id in data.Targets) {
                var target = GameObject.Find(id);
                if (target && !targets.Contains(target.GetComponent<Health>())) {
                    targets.Add(target.GetComponent<Health>());
                }
            }
            if (targets.Count == 0) { CompleteObjective(); }    //in case all targets were killed before objective started

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