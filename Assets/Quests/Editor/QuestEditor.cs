using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Quests
{
    public class QuestEditor : EditorWindow
    {
        Quest quest;    //the currently displayed Quest
        List<Node> nodes;
        List<(Node, Node)> links;
        Node linkParentNode = null;

        Rect canvas = new Rect(0, 0, 1000, 1000);
        Vector2 scrollPosition;
        Vector2 lastMousePosition;
        Vector2 dragOffset;

        [MenuItem("Window/Quest Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(QuestEditor), false, "Quest Editor");
        }

        private void OnEnable() {
            nodes = new List<Node>();
            links = new List<(Node, Node)>();

            Selection.selectionChanged += () => NewQuestSelected();
            NewQuestSelected();
        }

        void OnGUI() {
            if (!quest) return;

            scrollPosition = GUI.BeginScrollView(new Rect(Vector2.zero, position.size), scrollPosition, canvas);
            foreach (var node in nodes) { node.Draw(); }

            foreach (var link in links) {
                var parentNode = link.Item1;
                var childNode  = link.Item2;
                Handles.DrawBezier(parentNode.CentreBottom, childNode.CentreTop, parentNode.CentreBottom + Vector2.up * 10, childNode.CentreTop + Vector2.down * 10, Color.white, null, 3);
            }
            if (linkParentNode != null) {
                Handles.DrawBezier(linkParentNode.CentreBottom, Event.current.mousePosition, linkParentNode.CentreBottom + Vector2.up * 10, Event.current.mousePosition + Vector2.down * 10, Color.white, null, 3);
                GUI.changed = true;
            }

            GUI.EndScrollView();
            
            lastMousePosition = Event.current.mousePosition + scrollPosition;
            HandleMouseEvent(Event.current);

            if (GUI.changed) {
                EditorUtility.SetDirty(quest);
            }
            Repaint();
        }

        private void HandleMouseEvent(Event e) {
            if (ClickedOnNode(e)) { return; }
            if (e.type == EventType.ContextClick) {
                ShowContextMenu();
                e.Use();
            }
            if (e.type == EventType.MouseDown && linkParentNode != null) {
                linkParentNode = null;
                e.Use();
            }
        }

        #region nodeActions
        private bool ClickedOnNode(Event e) {
            foreach (Node node in nodes) {
                if (node.NodeArea.Contains(e.mousePosition + scrollPosition)) {
                    HandleNodeClick(node, e);
                    return true;
                }
            }
            return false;
        }
        private void HandleNodeClick(Node node, Event e) {
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) { LeftClickNode(node, e); }
                    if (e.button == 1) { RightClickNode(node, e); }
                    break;
                case EventType.MouseDrag:
                    DragNode(node, e);
                    break;
                case EventType.MouseUp:
                    StopDrag(node, e);
                    break;
            }
        }
        
        private void LeftClickNode(Node node, Event e) {
            if (linkParentNode != null) {
                if (linkParentNode != node) {
                    quest.AddLink(linkParentNode.objective, node.objective);
                    GUI.changed = true;
                }
                linkParentNode = null;
            } else {
                dragOffset = e.mousePosition - node.NodeArea.position;
            }
            e.Use();
        }
        private void DragNode(Node node, Event e) {
            if (dragOffset == Vector2.zero) { return; }

            node.Move(e.mousePosition - dragOffset);
            GUI.changed = true;
            e.Use();
        }
        private void StopDrag(Node node, Event e) {
            dragOffset = Vector2.zero;
            e.Use();
        }
        private void RightClickNode(Node node, Event e) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => quest.Delete(node.objective));
            menu.AddItem(new GUIContent("Create link"), false, () => linkParentNode = node);
            menu.AddItem(new GUIContent("Break links"), false, () => quest.BreakLinks(node.objective));
            menu.ShowAsContext();
            e.Use();
        }
        
        private Node FindObjectiveNode(int id) {
            foreach (var n in nodes) {
                if (n.objective.id == id) { return n; }
            }
            Debug.Log("No match found for " + id);
            return null;
        }
        #endregion

        private void ShowContextMenu() {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Objective/Kill"),     false, () => quest.AddObjective(new KillObjective(lastMousePosition)));
            menu.AddItem(new GUIContent("New Objective/Travel"),   false, () => quest.AddObjective(new TravelObjective(lastMousePosition)));
            menu.AddItem(new GUIContent("New Objective/Interact"), false, () => quest.AddObjective(new InteractObjective(lastMousePosition)));
            menu.ShowAsContext();
        }
        
        private void NewQuestSelected() {
            quest = (Selection.activeObject as Quest);
            if (!quest) { return; }

            quest.onLayoutChanged += Refresh;
            Refresh();
        }
        private void Refresh() {
            nodes.Clear();
            links.Clear();
            foreach (var objective in quest.Objectives.Values) {
                if (objective.id < 0) { objective.id = quest.GetNextObjectiveID(); }
                if (objective as KillObjective != null) {
                    nodes.Add(new KillObjectiveNode(objective));
                    continue;
                }
                if (objective as TravelObjective != null) {
                    nodes.Add(new TravelObjectiveNode(objective));
                    continue;
                }
                if (objective as InteractObjective != null) {
                    nodes.Add(new InteractObjectiveNode(objective));
                    continue;
                }
            }
            foreach (var link in quest.dependencies){
                links.Add((FindObjectiveNode(link.parentID), FindObjectiveNode(link.childID)));
            }
            Repaint();
        }
    }
}