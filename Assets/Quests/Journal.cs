using UnityEngine;

namespace RPG.Quests
{
    public class Journal : MonoBehaviour
    {
        [SerializeField] Quest quest;

        private void Start() {
            quest.Activate();
        }
    }
}