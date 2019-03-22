using UnityEngine;
using UnityEditor;

namespace RPG.Quests
{
    public class QuestEditor : EditorWindow
    {
        Quest quest;

        [MenuItem("Window/Quest Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(QuestEditor), false, "Quest Editor");
        }

        private void OnEnable() {
            Selection.selectionChanged += () => NewQuestSelected(Selection.activeObject as Quest);

            NewQuestSelected(Selection.activeObject as Quest);
        }

        private void NewQuestSelected(Quest quest) {
            if (!quest) { return; }
            quest.onChanged += Refresh;
            Refresh();
        }

        private void Refresh() {
            Repaint();
        }

        void OnGUI() {
            if (!quest) return;

            //scrollPosition = GUI.BeginScrollView(new Rect(Vector2.zero, position.size), scrollPosition, Canvas);

            foreach (var objective in quest.Objectives) {
                var node = new Node(objective); //TODO switch based on objective type
                node.Draw();
                //node.ProcessEvent(Event.current);
            }

            GUI.EndScrollView();

            //lastMousePosition = Event.current.mousePosition + scrollPosition;
            //ProcessEvent(Event.current);

            if (GUI.changed) {
                EditorUtility.SetDirty(quest);
                Repaint();
            }
        }
    }
}