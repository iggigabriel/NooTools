using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Noo.Tools.Editor
{
    public static class ScriptingDefineUtils
    {
        public static void EnableDefineForCurrentTarget(string define)
        {
            var activeBuild = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            PlayerSettings.GetScriptingDefineSymbols(activeBuild, out var defines);
            if (defines.Contains(define)) return;
            var newDefines = defines.Union(new[] {define}).ToArray();
            PlayerSettings.SetScriptingDefineSymbols(activeBuild, newDefines);
            AssetDatabase.Refresh();
        }

        public static void DisableDefineForCurrentTarget(string define)
        {
            var activeBuild = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            PlayerSettings.GetScriptingDefineSymbols(activeBuild, out var defines);
            if (!defines.Contains(define)) return;
            var newDefines = defines.Except(new[] { define }).ToArray();
            PlayerSettings.SetScriptingDefineSymbols(activeBuild, newDefines);
            AssetDatabase.Refresh();
        }
    }
}
