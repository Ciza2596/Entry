using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Entry.Editor
{
    public class EntryEditor : EditorWindow
    {
        private const string SPACE = "  ";

        private Vector3 _scrollPosition;

        //private method
        [MenuItem("Tools/CizaModule/Entry",priority = -200)]
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
            ShowUpdatable(rootObjectTypes);
            ShowFixedUpdatable(rootObjectTypes);
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

        private void ShowUpdatable(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(60);
            ShowVerticalInfo("Updatable", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetLifeScopeTypes(rootObjectType, out var lifeScopeTypes);
                    var tip = lifeScopeTypes.Contains(typeof(IUpdatable)) ? "O" : "x";
                    EditorGUILayout.LabelField(SPACE + tip, width);
                }
            }, width);
        }

        private void ShowFixedUpdatable(Type[] rootObjectTypes)
        {
            var width = GUILayout.Width(90);
            ShowVerticalInfo("FixedUpdatable", () =>
            {
                foreach (var rootObjectType in rootObjectTypes)
                {
                    Entry.TryGetLifeScopeTypes(rootObjectType, out var lifeScopeTypes);
                    var tip = lifeScopeTypes.Contains(typeof(IFixedUpdatable)) ? "O" : "x";
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
                    Entry.TryGetLifeScopeTypes(rootObjectType, out var lifeScopeTypes);
                    var tip = lifeScopeTypes.Contains(typeof(IReleasable)) ? "O" : "x";
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