using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Combat;

namespace RPG.Quests
{
    [CreateAssetMenu(menuName = "RPG/Quests/Objectives/Kill Objective")]
    public class KillObjective : Objective
    {
        private List<ObjectiveTarget> targets;

        public override void Activate() {
            targets = new List<ObjectiveTarget>();
            var allTargets = FindObjectsOfType<ObjectiveTarget>();
            foreach (var target in allTargets) {
                if (target.objective == this) { targets.Add(target); }
            }
            Assert.IsTrue(targets.Count > 0, "No targets assigned for this objective: " + name);

            foreach (var target in targets) {
                target.GetComponent<Health>().onDeath += () => KilledTarget(target);
            }
        }

        private void KilledTarget(ObjectiveTarget target) {
            targets.Remove(target);
            if (targets.Count == 0) { CompleteObjective(); }
        }
    }
}