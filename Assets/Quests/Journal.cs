using UnityEngine;

namespace RPG.Quests
{
    [ExecuteInEditMode]
    public class Journal : MonoBehaviour
    {
        [SerializeField] Quest quest;

        private void Start() {
            quest.onChanged += () => print(quest.objectives.Count);
        }
    }
}