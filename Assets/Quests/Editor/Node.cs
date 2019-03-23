﻿using System;
using UnityEngine;
using UnityEditor;
using RPG.Combat;

namespace RPG.Quests
{
    public class Node
    {
        public Objective objective { get; private set; }

        public Rect NodeArea => new Rect(objective.position, size);
        private Vector2 size = new Vector2(200, 150);
        private GUIStyle style = new GUIStyle();

        public Node(Objective objective) {
            this.objective = objective;

            style.normal.background = SelectNodeColor();
            style.border = new RectOffset(12, 12, 12, 12);
            style.padding = new RectOffset(20, 20, 20, 20);
        }

        public void Draw() {
            GUILayout.BeginArea(NodeArea, style);
            var textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;

            if ((objective as KillObjective) != null) {
                DrawProperties((objective as KillObjective), textStyle);
            }
            if ((objective as TravelObjective) != null) {
                DrawProperties((objective as TravelObjective), textStyle);
            }
            GUILayout.EndArea();
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

        private void DrawProperties(KillObjective objective, GUIStyle textStyle) {
            objective.Target = EditorGUILayout.ObjectField(objective.Target, typeof(Health), true) as Health;
        }
        private void DrawProperties(TravelObjective objective, GUIStyle textStyle) {
            objective.Destination = EditorGUILayout.TextArea(objective.Destination, textStyle);
        }

        public void Move(Vector2 position) {
            objective.position = position;
        }
    }
}