using UnityEngine;
using UnityEditor;
using System;

namespace RPG.Quests
{
    public class QuestEditor : EditorWindow
    {
        Quest quest;    //the currently displayed Quest

        Rect canvas = new Rect(0, 0, 2000, 2000);
        Vector2 scrollPosition;
        Vector2 lastMousePosition;

        [MenuItem("Window/Quest Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(QuestEditor), false, "Quest Editor");
        }

        private void OnEnable() {
            Selection.selectionChanged += () => NewQuestSelected();

            NewQuestSelected();
        }

        private void NewQuestSelected() {
            quest = (Selection.activeObject as Quest);
            if (!quest) { return; }

            quest.onChanged += Refresh;
            Refresh();
        }

        private void Refresh() {
            Repaint();
        }

        void OnGUI() {
            if (!quest) return;

            scrollPosition = GUI.BeginScrollView(new Rect(Vector2.zero, position.size), scrollPosition, canvas);

            foreach (var objective in quest.Objectives) {
                var node = new Node(objective);
                node.Draw();
                //node.ProcessEvent(Event.current);
            }

            GUI.EndScrollView();

            lastMousePosition = Event.current.mousePosition + scrollPosition;
            HandleMouseEvent(Event.current);

            if (GUI.changed) {
                EditorUtility.SetDirty(quest);
                Repaint();
            }
        }

        private void HandleMouseEvent(Event e) {
            switch (e.type) {
                //Right click
                case EventType.ContextClick:
                    ShowContextMenu();
                    e.Use();
                    break;
            }
        }
        private void ShowContextMenu() {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Objective/Kill"), false,   () => quest.AddObjective(new KillObjective(lastMousePosition)));
            menu.AddItem(new GUIContent("New Objective/Travel"), false, () => quest.AddObjective(new TravelObjective(lastMousePosition)));
            menu.ShowAsContext();
        }
    }
}