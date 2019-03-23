using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Quests
{
    public class QuestEditor : EditorWindow
    {
        Quest quest;    //the currently displayed Quest
        List<Node> nodes;

        Rect canvas = new Rect(0, 0, 2000, 2000);
        Vector2 scrollPosition;
        Vector2 lastMousePosition;
        Vector2 dragOffset;

        [MenuItem("Window/Quest Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(QuestEditor), false, "Quest Editor");
        }

        private void OnEnable() {
            nodes = new List<Node>();

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
            nodes.Clear();
            foreach (var objective in quest.objectives) {
                nodes.Add(new Node(objective));
            }
            Repaint();
        }

        void OnGUI() {
            if (!quest) return;

            scrollPosition = GUI.BeginScrollView(new Rect(Vector2.zero, position.size), scrollPosition, canvas);
            foreach (var node in nodes) { node.Draw(); }
            GUI.EndScrollView();

            lastMousePosition = Event.current.mousePosition + scrollPosition;
            HandleMouseEvent(Event.current);

            if (GUI.changed) {
                EditorUtility.SetDirty(quest);
                Repaint();
            }
        }

        private void HandleMouseEvent(Event e) {
            //node click
            foreach (Node node in nodes) {
                if (node.NodeArea.Contains(e.mousePosition)) {
                    ClickNode(node, e);
                    return;
                }
            }
            //background click
            if (e.type == EventType.ContextClick) {
                ShowContextMenu();
                e.Use();
            }
        }
        private void ClickNode(Node node, Event e) {
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) { LeftClickNode(node, e); }
                    if (e.button == 1) { RightClickNode(node, e); }
                    break;
                case EventType.MouseDrag:
                    DragNode(node, e);
                    break;
                case EventType.MouseUp:
                    //MouseUp(e);
                    break;
            }
        }
        
        private void LeftClickNode(Node node, Event e) {
            //if editor has open link
                //create link from link origin to this node
                //editor open link = null
                //GUI.changed = true;
            //else (no open link)
            dragOffset = e.mousePosition - node.NodeArea.position;
            e.Use();
        }
        private void RightClickNode(Node node, Event e) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => quest.Delete(node.objective));
            //menu.AddItem(new GUIContent("Add connection"), false, () => StartLinking());
            //menu.AddItem(new GUIContent("Break child connections"), false, () => BreakChildConnections());
            menu.ShowAsContext();
            e.Use();
        }
        private void DragNode(Node node, Event e) {
            if (dragOffset == Vector2.zero) { return; }

            node.Move(e.mousePosition - dragOffset);
            e.Use();
            GUI.changed = true;
        }
        private void StopDrag(Node node, Event e) {
            dragOffset = Vector2.zero;
            e.Use();
        }

        private void ShowContextMenu() {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Objective/Kill"), false,   () => quest.AddObjective(new KillObjective(lastMousePosition)));
            menu.AddItem(new GUIContent("New Objective/Travel"), false, () => quest.AddObjective(new TravelObjective(lastMousePosition)));
            menu.ShowAsContext();
        }
    }
}