using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.Combat;

namespace RPG.Quests
{
    public class KillObjective : Objective
    {
        [Header("Targets to Kill")]
        [SerializeField] List<Health> targets;

        public override void Activate() {
            Assert.IsTrue(targets.Count > 0, "No targets assigned for this objective: " + name);

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