using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace CizaEntry.Editor
{
    public class EntryEditor : EditorWindow
    {
        private const string SPACE = "  ";

        private Vector3 _scrollPosition;

        //private method
        [MenuItem("Tools/Ciza/Entry", priority = -200)]
        private static void ShowWindow() =>
            GetWindow<EntryEditor>("Entry");

        private void OnGUI()
        {
            if (!Entry.IsInitialized)
            {
                EditorGUILayout.LabelField("Entry isn't initialized.");
                return;
            }

            ShowDetail();
        }

        private void ShowDetail()
        {
            var instanceTypes = Entry.InstanceTypes;

            EditorGUILayout.BeginHorizontal();

            ShowNumber(instanceTypes);
            ShowInstance(instanceTypes);
            ShowFixedTickable(instanceTypes);
            ShowTickable(instanceTypes);
            ShowLateTickable(instanceTypes);
            ShowDisposable(instanceTypes);
            ShowKeys(instanceTypes);

            EditorGUILayout.EndHorizontal();
        }

        private void ShowNumber(Type[] instanceTypes)
        {
            var width = GUILayout.Width(20);
            ShowVerticalInfo("", () =>
            {
                var length = instanceTypes.Length;
                for (var i = 1; i <= length; i++)
                    EditorGUILayout.LabelField(i.ToString() + ": ", width);
            }, width);
        }

        private void ShowInstance(Type[] instanceTypes)
        {
            var width = GUILayout.Width(150);
            ShowVerticalInfo("Instance", () =>
            {
                foreach (var instanceType in instanceTypes)
                    EditorGUILayout.LabelField(instanceType.Name, width);
            }, width);
        }

        private void ShowFixedTickable(Type[] instanceTypes)
        {
            var width = GUILayout.Width(90);
            ShowVerticalInfo("FixedTickable", () =>
            {
                foreach (var instanceType in instanceTypes)
                {
                    Entry.TryGetEntryPoints(instanceType, out var entryPoints);
                    var tip = entryPoints.Contains(typeof(IFixedTickable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowTickable(Type[] instanceTypes)
        {
            var width = GUILayout.Width(60);
            ShowVerticalInfo("Tickable", () =>
            {
                foreach (var instanceType in instanceTypes)
                {
                    Entry.TryGetEntryPoints(instanceType, out var entryPoints);
                    var tip = entryPoints.Contains(typeof(ITickable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowLateTickable(Type[] instanceTypes)
        {
            var width = GUILayout.Width(75);
            ShowVerticalInfo("LateTickable", () =>
            {
                foreach (var instanceType in instanceTypes)
                {
                    Entry.TryGetEntryPoints(instanceType, out var entryPoints);
                    var tip = entryPoints.Contains(typeof(ILateTickable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowDisposable(Type[] instanceTypes)
        {
            var width = GUILayout.Width(67);
            ShowVerticalInfo("Disposable", () =>
            {
                foreach (var instanceType in instanceTypes)
                {
                    Entry.TryGetEntryPoints(instanceType, out var entryPoints);
                    var tip = entryPoints.Contains(typeof(IDisposable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowKeys(Type[] instanceTypes)
        {
            ShowVerticalInfo("Keys", () =>
            {
                foreach (var instanceType in instanceTypes)
                {
                    Entry.TryGetKeys(instanceType, out var keys);

                    var instanceTypeNames = string.Empty;
                    foreach (var key in keys)
                        instanceTypeNames += key.Name + ", ";

                    EditorGUILayout.LabelField(instanceTypeNames);
                }
            });
        }

        private void ShowVerticalInfo(string label, Action action, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(label, options);
            action();

            EditorGUILayout.EndVertical();
        }
    }
}