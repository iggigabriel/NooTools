#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Noo.Tools
{
    [InitializeOnLoad]
    public class SoftFloatEditorWindow : EditorWindow
    {
        public Sfloat sfVal;
        public int intVal;

        public Sdouble sdVal;
        public long longVal;

        SerializedObject so;

        [MenuItem("Tools/Soft Float/Calculator")]
        static void Init()
        {
            var window = GetWindow<SoftFloatEditorWindow>();
            window.titleContent = new GUIContent("Soft Float Calculator");
            window.Show();
        }

        static SoftFloatEditorWindow()
        {
#if SOFT_FLOAT_SAFETY_CHECKS
            EditorApplication.delayCall += () => Menu.SetChecked("Tools/Soft Float/Safety Checks", true);
#endif
        }

#if SOFT_FLOAT_SAFETY_CHECKS
        [MenuItem("Tools/Soft Float/Safety Checks")]
        static void EnableSafetyChecks()
        {
            DisableDefineForCurrentTarget("SOFT_FLOAT_SAFETY_CHECKS");
            Menu.SetChecked("Tools/Soft Float/Safety Checks", false);
        }
#else
        [MenuItem("Tools/Soft Float/Safety Checks")]
        static void EnableSafetyChecks()
        {
            EnableDefineForCurrentTarget("SOFT_FLOAT_SAFETY_CHECKS");
            Menu.SetChecked("Tools/Soft Float/Safety Checks", true);
        }
#endif

        static void EnableDefineForCurrentTarget(string define)
        {
            var activeBuild = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            PlayerSettings.GetScriptingDefineSymbols(activeBuild, out var defines);
            if (defines.Contains(define)) return;
            var newDefines = defines.Union(new[] { define }).ToArray();
            PlayerSettings.SetScriptingDefineSymbols(activeBuild, newDefines);
            AssetDatabase.Refresh();
        }

        static void DisableDefineForCurrentTarget(string define)
        {
            var activeBuild = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            PlayerSettings.GetScriptingDefineSymbols(activeBuild, out var defines);
            if (!defines.Contains(define)) return;
            var newDefines = defines.Except(new[] { define }).ToArray();
            PlayerSettings.SetScriptingDefineSymbols(activeBuild, newDefines);
            AssetDatabase.Refresh();
        }

        void Dispose()
        {
            if (so != null)
            {
                so.Dispose();
                so = null;
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        void OnGUI()
        {
            if (so != null && !so.targetObject) Dispose();
            so ??= new SerializedObject(this);

            so.Update();

            var intProp = so.FindProperty("intVal");
            var sfProp = so.FindProperty("sfVal");
            var sfPropRaw = so.FindProperty("sfVal.Raw");
            var longProp = so.FindProperty("longVal");
            var sdProp = so.FindProperty("sdVal");
            var sdPropRaw = so.FindProperty("sdVal.Raw");

            GUILayout.Space(16f);

            GUILayout.Label("Soft Float", EditorStyles.boldLabel);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(sfProp);

                if (check.changed) intProp.intValue = sfPropRaw.intValue;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(intProp);

                if (check.changed) sfPropRaw.intValue = intProp.intValue;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var hexVal = EditorGUILayout.TextField("Hex Val", $"0x{sfPropRaw.intValue:X}");

                if (check.changed)
                {
                    if (!hexVal.StartsWith("0x")) hexVal = $"0x{hexVal}";

                    try
                    {
                        intProp.intValue = sfPropRaw.intValue = Convert.ToInt32(hexVal, 16);
                    }
                    catch
                    {
                    }
                }
            }

            GUILayout.Space(16f);

            GUILayout.Label("Soft Double", EditorStyles.boldLabel);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(sdProp);

                if (check.changed) longProp.longValue = sdPropRaw.longValue;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(longProp);

                if (check.changed) sdPropRaw.longValue = longProp.longValue;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var hexVal = EditorGUILayout.TextField("Hex Val", $"0x{sdPropRaw.intValue:X}");

                if (check.changed)
                {
                    if (!hexVal.StartsWith("0x")) hexVal = $"0x{hexVal}";

                    try
                    {
                        longProp.longValue = sdPropRaw.longValue = Convert.ToInt64(hexVal, 16);
                    }
                    catch
                    {
                    }
                }
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif