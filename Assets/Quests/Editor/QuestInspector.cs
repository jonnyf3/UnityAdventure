using UnityEditor;

namespace RPG.Quests
{
    [CustomEditor(typeof(Quest))]
    public class QuestInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            Quest quest = (Quest)target;

            quest.questName = EditorGUILayout.TextField("Quest Name", quest.questName);
            quest.experiencePoints = EditorGUILayout.IntField("Experience", quest.experiencePoints);

            //TODO button to open quest editor window?
        }
    }
}