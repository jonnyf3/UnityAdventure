using UnityEditor;

namespace RPG.Saving
{
    [CustomEditor(typeof(SaveableEntity))]
    public class SaveableEntityInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var se = (SaveableEntity)target;
            EditorGUILayout.LabelField("GUID:", se.GUID);
        }
    }
}