using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Combat;

namespace RPG.Quests
{
    public abstract class Node
    {
        public Objective objective { get; }

        public Rect NodeArea => new Rect(objective.position, size);
        protected Vector2 size = new Vector2(200, 150);
        protected GUIStyle style = new GUIStyle();

        public Node(Objective objective) {
            this.objective = objective;

            style.normal.background = SetBackgroundColour();
            style.border = new RectOffset(12, 12, 12, 12);
            style.padding = new RectOffset(20, 20, 20, 20);
        }

        public void Move(Vector2 position) {
            objective.position = position;
        }

        public void Draw() {
            GUILayout.BeginArea(NodeArea, style);
            var textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;

            DrawProperties(objective, textStyle);

            GUILayout.EndArea();
        }

        protected abstract void DrawProperties(Objective objective, GUIStyle textStyle);
        protected abstract Texture2D SetBackgroundColour();
        //node0 = dark grey
        //node1 = blue          => TravelObjectiveNode
        //node2 = green
        //node3 = dark green
        //node4 = yellow
        //node5 = orange/brown
        //node6 = dark red      => KillObjectiveNode
    }

    public class KillObjectiveNode : Node
    {
        //Kill Objective Node
        private int targetCount = 0;
        private List<Health> targets;
        
        public KillObjectiveNode(Objective objective) : base(objective) { }

        protected override Texture2D SetBackgroundColour() {
            return EditorGUIUtility.Load("node6") as Texture2D;  //dark red
        }

        protected override void DrawProperties(Objective o, GUIStyle textStyle) {
            var objective = (o as KillObjective);
            if (targets == null && targetCount == 0) {
                LoadTargets(objective);
            }

            EditorGUILayout.LabelField("      KILL OBJECTIVE", new GUIStyle(EditorStyles.boldLabel));

            objective.description = EditorGUILayout.TextArea(objective.description, new GUIStyle(EditorStyles.textArea));

            EditorGUIUtility.labelWidth = 80f;
            targetCount = Mathf.Clamp(EditorGUILayout.IntField("Targets", targetCount), 1, 1000);
            if (targets.Count > targetCount) { targets = targets.GetRange(0, targetCount); }
            size = new Vector2(200, 100 + targetCount * 20);

            //TODO add padding?
            for (int i = 0; i < targetCount; i++) {
                if (i >= targets.Count) {
                    targets.Add(EditorGUILayout.ObjectField(null, typeof(Health), true) as Health);
                }
                else {
                    targets[i] = EditorGUILayout.ObjectField(targets[i], typeof(Health), true) as Health;
                }
            }

            objective.Targets = new List<string>();
            foreach (var target in targets) {
                if (!target) { objective.Targets.Add(""); }
                else { objective.Targets.Add(target.name); }    //TODO use GUID not name
            }
        }
        private void LoadTargets(KillObjective objective) {
            targets = new List<Health>();
            targetCount = objective.Targets.Count;
            foreach (var id in objective.Targets) {
                if (id != "") { targets.Add(GameObject.Find(id).GetComponent<Health>()); }
            }
        }
    }

    public class TravelObjectiveNode : Node
    {
        public TravelObjectiveNode(Objective objective) : base(objective) { }

        protected override Texture2D SetBackgroundColour() {
            return EditorGUIUtility.Load("node1") as Texture2D;  //blue
        }
        
        protected override void DrawProperties(Objective o, GUIStyle textStyle) {
            var objective = o as TravelObjective;
            objective.Destination = EditorGUILayout.TextArea(objective.Destination, textStyle);
        }
    }
}