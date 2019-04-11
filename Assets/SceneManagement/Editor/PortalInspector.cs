using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace RPG.SceneManagement
{
    [CustomEditor(typeof(Portal))]
    public class PortalInspector : Editor
    {
        private Portal portal;
        private string[] scenes;
        private int sceneIndex = 0;

        private void OnEnable() {
            portal = (Portal)target;
            scenes = GetSceneNames();
        }

        public override void OnInspectorGUI() {
            Undo.RecordObject(portal, "changes to Portal");

            EditorGUILayout.LabelField("Identifier", EditorStyles.boldLabel);
            portal.identifier = (Portal.PortalIdentifier)EditorGUILayout.EnumPopup("Identifier", portal.identifier);

            EditorGUILayout.LabelField("Destination", EditorStyles.boldLabel);
            sceneIndex = EditorGUILayout.Popup("Target Scene", sceneIndex, scenes);
            portal.sceneToLoad = scenes[sceneIndex];
            portal.targetPortal = (Portal.PortalIdentifier)EditorGUILayout.EnumPopup("Target Identifier", portal.targetPortal);
        }

        private string[] GetSceneNames() {
            List<string> scenePaths = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                if (scene.enabled) { scenePaths.Add(scene.path); }
            }

            //Exclude _Main (at build index 0) from possible destinations
            string[] scenes = new string[scenePaths.Count - 1];
            for (int i = 0; i < scenes.Length; i++) {
                var sceneName = Path.GetFileNameWithoutExtension(scenePaths[i + 1]);
                scenes[i] = sceneName;
                if (sceneName == portal.sceneToLoad) { sceneIndex = i; }
            }

            return scenes;
        }
    }
}