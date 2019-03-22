using UnityEngine;

namespace RPG.Quests
{
    public abstract class ObjectiveBehaviour : MonoBehaviour
    {
        private Objective objective;
        public virtual void Setup(Objective objectiveData) {
            objective = objectiveData;
        }
        
        protected void CompleteObjective() {
            print("Objective complete!");
            objective.Complete();
            Destroy(this);
        }
    }
}