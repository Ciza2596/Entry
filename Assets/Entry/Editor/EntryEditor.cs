using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Cilix.Editor
{
    public class EntryEditor : EditorWindow
    {
        private const string SPACE = "  ";

        private Vector3 _scrollPosition;

        //private method
        [MenuItem("Tools/Cilix/Entry",priority = -200)]
        private static void ShowWindow() =>
            GetWindow<EntryEditor>("Entry");

        private void OnGUI()
        {
            if (!Entry.IsInitialized)
            {
                EditorGUILayout.LabelField("Entry isnt initialized.");
                return;
            }

            ShowDetail();
        }

        private void ShowDetail()
        {
            var rootObjectTypes = Entry.RootObjectTypes;

            EditorGUILayout.BeginHorizontal();

            ShowNumber(rootObjectTypes);
            ShowRootObject(rootObjectTypes);
            ShowFixedTickable(rootObjectTypes);
            ShowTickable(rootObjectTypes);
            ShowLateTickable(rootObjectTypes);
            ShowReleasable(rootObjectTypes);
            ShowRegisteredObjectTypes(rootObjectTypes);

            EditorGUILayout.EndHorizontal();
        }

        private void ShowNumber(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(20);
            ShowVerticalInfo("", () =>
            {
                var length = rootObjectTypes.Length;
                for (var i = 1; i <= length; i++)
                    EditorGUILayout.LabelField(i.ToString() + ": ", width);
            }, width);
        }

        private void ShowRootObject(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(150);
            ShowVerticalInfo("RootObject", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                    EditorGUILayout.LabelField(rootObjectType.Name, width);
            }, width);
        }

        private void ShowFixedTickable(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(90);
            ShowVerticalInfo("FixedTickable", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetEntryPointTypes(rootObjectType, out var entryPointTypes);
                    var tip = entryPointTypes.Contains(typeof(IFixedTickable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }
        
        private void ShowTickable(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(60);
            ShowVerticalInfo("Tickable", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetEntryPointTypes(rootObjectType, out var entryPointTypes);
                    var tip = entryPointTypes.Contains(typeof(ITickable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }
        
        private void ShowLateTickable(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(75);
            ShowVerticalInfo("LateTickable", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetEntryPointTypes(rootObjectType, out var entryPointTypes);
                    var tip = entryPointTypes.Contains(typeof(ILateTickable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowReleasable(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(67);
            ShowVerticalInfo("Releasable", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetEntryPointTypes(rootObjectType, out var entryPointTypes);
                    var tip = entryPointTypes.Contains(typeof(IReleasable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowRegisteredObjectTypes(Type[] rootObjectTypes)
        {
            ShowVerticalInfo("RegisteredObjectTypes", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetRegisteredObjectTypes(rootObjectType, out var registeredTypes);

                    var rootObjectTypeNames = string.Empty;
                    foreach (var registeredType in registeredTypes)
                        rootObjectTypeNames += registeredType.Name + ", ";

                    EditorGUILayout.LabelField(rootObjectTypeNames);
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