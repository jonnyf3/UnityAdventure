using UnityEngine;

namespace RPG.Quests
{
    public abstract class ObjectiveBehaviour : MonoBehaviour
    {
        public abstract void Setup(Objective objectiveData);
        
        protected void CompleteObjective() {
            print("Objective complete!");
            Destroy(this);
        }
    }
}