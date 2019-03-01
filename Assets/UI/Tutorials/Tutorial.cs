using UnityEngine;

namespace RPG.UI
{
    [CreateAssetMenu(menuName = "RPG/UI/Tutorial")]
    public class Tutorial : ScriptableObject
    {
        public string title;
        [TextArea(2,8)] public string description;
    }
}