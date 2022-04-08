using System.Collections.Generic;
using System.Linq;
using Assets.Source.Scripts.BlackJack;
using Assets.Source.Scripts.Cards;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomEditors
{
    [CustomEditor(typeof(GameScript))]
    internal class GameScriptEditor : UnityEditor.Editor
    {
        private GameScript script;

        void OnEnable()
        {
            script = target as GameScript;

            script.manager.OnTaskAdd -= Manager_OnTaskAdd;
            script.manager.OnTaskAdd += Manager_OnTaskAdd;
        }

        private void Manager_OnTaskAdd(TaskItem task)
        {
            Repaint();

            var info = tasksInfo.FirstOrDefault(x => x.task == task);

            if (info != null)
                return;

            info = new TaskInfo { task = task, onAdd = Time.time };

            tasksInfo.Push(info);

            task.OnStart += (x) => info.onStart = Time.time;
            task.OnEnd += (x) => info.onEnd = Time.time;

            Repaint();
        }

        private Vector2 scroll = new Vector2();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Task Quele c:{tasksInfo.Count} a:{tasksInfo.Where(x => x.onEnd == null).Count()}");
            GUILayout.Label($"Time: {Time.time}");
            GUILayout.EndHorizontal();
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(300));
            foreach (var info in tasksInfo)
            {
                GUILayout.BeginHorizontal();
                const float timeSize = 50;
                GUILayout.Label(info.task.Name, GUILayout.Width(EditorGUIUtility.currentViewWidth - timeSize * 3 - 70));
                GUILayout.Label($"{info.onAdd:000.00}", GUILayout.Width(timeSize));
                GUILayout.Label($"{info.onStart:000.00}", GUILayout.Width(timeSize));
                GUILayout.Label($"{info.onEnd:000.00}", GUILayout.Width(timeSize));
                GUILayout.EndHorizontal();

            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private Stack<TaskInfo> tasksInfo = new Stack<TaskInfo>();

        private class TaskInfo
        {
            public TaskItem task;
            public float? onAdd;
            public float? onStart;
            public float? onEnd;
        }
    }
}
