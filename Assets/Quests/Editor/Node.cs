using System;
using UnityEngine;
using UnityEditor;

namespace RPG.Quests
{
    public class Node
    {
        private Objective objective;

        private GUIStyle style = new GUIStyle();
        private Vector2 size = new Vector2(300, 100);

        public Node(Objective objective) {
            this.objective = objective;

            style.normal.background = SelectNodeColor();
            style.border = new RectOffset(12, 12, 12, 12);
            style.padding = new RectOffset(20, 20, 20, 20);
        }

        private Texture2D SelectNodeColor() {
            if ((objective as KillObjective) != null) {
                return EditorGUIUtility.Load("node6") as Texture2D;  //dark red
            }
            if ((objective as TravelObjective) != null) {
                return EditorGUIUtility.Load("node1") as Texture2D;  //blue
            }
            //TODO add cases for other objective types
            //node0 = dark grey
            //node2 = green
            //node3 = dark green
            //node4 = yellow
            //node5 = orange/brown
            throw new TypeAccessException("Unknown objective type");
        }

        public void Draw() {
            GUILayout.BeginArea(GetRect(), style);
            var textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;
            //nodeModel.text = EditorGUILayout.TextArea(nodeModel.text, textStyle);
            //nodeModel.actionToTrigger = EditorGUILayout.TextArea(nodeModel.actionToTrigger, textStyle);
            GUILayout.EndArea();
        }

        private Rect GetRect() {
            return new Rect(objective.position, size);
        }
    }
}