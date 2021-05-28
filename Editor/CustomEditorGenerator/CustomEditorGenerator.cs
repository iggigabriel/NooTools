using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace Nootools.Editor
{
	public static class CustomEditorGenerator
    {
		[MenuItem("Assets/Create/Nootools/Editor From Selected Script")]
		public static void GenerateCustomEditorFromSelectedScript()
		{
			if (!GenerateCustomEditorFromSelectedScript_Validator())
			{
				return;
			}

			MonoScript scriptAsset = Selection.objects[0] as MonoScript;
			Type scriptClass = scriptAsset.GetClass();
			var editorClass = scriptClass.Name + "Editor";

			if (Type.GetType(editorClass) != null)
			{
				if (!EditorUtility.DisplayDialog("Create Script Editor?",
					"Editor for class “" + scriptClass.Name + "“ already exists. Are you sure you create and possibly overwrite it?", "Create", "Cancel"))
				{
					return;
				}
			}

			var scriptDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(scriptAsset));
			
			var editorDir = scriptDir + Path.DirectorySeparatorChar + "Editor";
			var editorPath = editorDir + Path.DirectorySeparatorChar + scriptClass.Name + "Editor.cs";

			if (scriptDir.Contains("Runtime"))
            {
				editorDir = scriptDir.Replace("Runtime", "Editor");
				editorPath = Path.Combine(editorDir, scriptClass.Name + "Editor.cs");
			}

			if (!Directory.Exists(editorDir))
			{
				Directory.CreateDirectory(editorDir);
			}

			string editorContents = GenerateScriptEditor(scriptClass);

			File.WriteAllText(editorPath, editorContents);

			AssetImporter.GetAtPath(editorPath);
			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/Create/Nootools/Editor From Selected Script", true)]
		public static bool GenerateCustomEditorFromSelectedScript_Validator()
		{
			if (Selection.objects != null &&
				Selection.objects.Length == 1 &&
				Selection.objects[0] is MonoScript &&
				(Selection.objects[0] as MonoScript).GetClass() != null &&
				!(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(UnityEditor.Editor)) &&
				(
					(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(ScriptableObject)) ||
					(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(MonoBehaviour))
				)
			)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static string GenerateScriptEditor(Type type)
		{
			StringBuilder builder = new StringBuilder();

			var editorClass = type.Name + "Editor";

			List<FieldInfo> visibleFields = new List<FieldInfo>();

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields)
			{
				if (field.IsStatic || field.IsInitOnly || field.IsLiteral)
				{
					continue;
				}

				if (!field.IsNotSerialized && field.IsPublic)
				{
					visibleFields.Add(field);
					continue;
				}

				if (field.IsPrivate && field.GetCustomAttributes(typeof(SerializeField), true).Length > 0)
				{
					visibleFields.Add(field);
					continue;
				}
			}

			builder.AppendLine("using UnityEngine;");
			builder.AppendLine("using UnityEditor;");
			builder.AppendLine("using System;");
			builder.AppendLine("using System.Collections;");
			builder.AppendLine("using System.Collections.Generic;");

			if (!String.IsNullOrEmpty(type.Namespace))
			{
				builder.AppendLine(String.Format("using {0};", type.Namespace));
			}

			builder.AppendLine();

			builder.AppendLine(String.Format("[CustomEditor(typeof({0}))]", type.Name));
			builder.AppendLine(String.Format("public class {0} : Editor", editorClass));
			builder.AppendLine("{");
			builder.AppendLine();

			foreach (FieldInfo field in visibleFields)
			{
				builder.AppendLine(String.Format("\tSerializedProperty sp{0};", UppercaseFirst(field.Name)));
			}


			// FUNCTION: OnEnable()
			builder.AppendLine();

			builder.AppendLine("\tvoid OnEnable()");
			builder.AppendLine("\t{");

			foreach (FieldInfo field in visibleFields)
			{
				builder.AppendLine(String.Format("\t\tsp{0} = serializedObject.FindProperty(\"{1}\");", UppercaseFirst(field.Name), field.Name));
			}

			builder.AppendLine("\t}");
			// END FUNCTION


			// FUNCTION: OnInspectorGUI()
			builder.AppendLine();

			builder.AppendLine("\tpublic override void OnInspectorGUI()");
			builder.AppendLine("\t{");
			builder.AppendLine();
			builder.AppendLine(String.Format("\t\t{0} instance = target as {0};", type.Name));

			builder.AppendLine();

			builder.AppendLine("\t\tserializedObject.Update();");
			builder.AppendLine("\t\tEditorGUILayout.Space();");
			builder.AppendLine();

			foreach (FieldInfo field in visibleFields)
			{
				builder.AppendLine(String.Format("\t\tEditorGUILayout.PropertyField (sp{0}, true);", UppercaseFirst(field.Name)));
				builder.AppendLine();
			}

			builder.AppendLine();
			builder.AppendLine("\t\tserializedObject.ApplyModifiedProperties();");


			builder.AppendLine("\t}");
			// END FUNCTION


			// FUNCTION: OnSceneGUI()
			builder.AppendLine();
			builder.AppendLine("\t/*");

			builder.AppendLine("\tpublic void OnSceneGUI()");
			builder.AppendLine("\t{");

			builder.AppendLine("\t}");
			builder.AppendLine("\t*/");



			builder.AppendLine();
			builder.AppendLine("}");
			// END FUNCTION

			return builder.ToString();
		}


		[MenuItem("Assets/Create/Nootools/Default Editor From Selected Script")]
		public static void GenerateDefaultEditorFromSelectedScript()
		{
			if (!GenerateDefaultEditorFromSelectedScript_Validator())
			{
				return;
			}

			MonoScript scriptAsset = Selection.objects[0] as MonoScript;
			Type scriptClass = scriptAsset.GetClass();
			var editorClass = scriptClass.Name + "Editor";

			if (System.Type.GetType(editorClass) != null)
			{
				if (!EditorUtility.DisplayDialog("Create Script Editor?",
					"Editor for class “" + scriptClass.Name + "“ already exists. Are you sure you create and possibly overwrite it?", "Create", "Cancel"))
				{
					return;
				}
			}

			var scriptDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(scriptAsset));
			var editorDir = scriptDir + Path.DirectorySeparatorChar + "Editor";
			var editorPath = editorDir + Path.DirectorySeparatorChar + scriptClass.Name + "Editor.cs";

			if (scriptDir.Contains("Runtime"))
			{
				editorDir = scriptDir.Replace("Runtime", "Editor");
				editorPath = Path.Combine(editorDir, scriptClass.Name + "Editor.cs");
			}

			if (!Directory.Exists(editorDir))
			{
				Directory.CreateDirectory(editorDir);
			}

			string editorContents = GenerateScriptDefaultEditor(scriptClass);

			File.WriteAllText(editorPath, editorContents);

			AssetImporter.GetAtPath(editorPath);
			AssetDatabase.Refresh();
		}

		[MenuItem("Assets/Create/Nootools/Default Editor From Selected Script", true)]
		public static bool GenerateDefaultEditorFromSelectedScript_Validator()
		{
			if (Selection.objects != null &&
				Selection.objects.Length == 1 &&
				Selection.objects[0] is MonoScript &&
				(Selection.objects[0] as MonoScript).GetClass() != null &&
				!(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(UnityEditor.Editor)) &&
				(
					(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(ScriptableObject)) ||
					(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(MonoBehaviour))
				)
			)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static string GenerateScriptDefaultEditor(Type type)
		{
			StringBuilder builder = new StringBuilder();

			var editorClass = type.Name + "Editor";

			List<FieldInfo> visibleFields = new List<FieldInfo>();

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields)
			{
				if (field.IsStatic || field.IsInitOnly || field.IsLiteral)
				{
					continue;
				}

				if (!field.IsNotSerialized && field.IsPublic)
				{
					visibleFields.Add(field);
					continue;
				}

				if (field.IsPrivate && field.GetCustomAttributes(typeof(SerializeField), true).Length > 0)
				{
					visibleFields.Add(field);
					continue;
				}
			}

			builder.AppendLine("using UnityEngine;");
			builder.AppendLine("using UnityEditor;");
			builder.AppendLine("using System;");
			builder.AppendLine("using System.Collections;");
			builder.AppendLine("using System.Collections.Generic;");

			if (!String.IsNullOrEmpty(type.Namespace))
			{
				builder.AppendLine(String.Format("using {0};", type.Namespace));
			}

			builder.AppendLine();

			builder.AppendLine(String.Format("[CustomEditor(typeof({0}))]", type.Name));
			builder.AppendLine(String.Format("public class {0} : Editor", editorClass));
			builder.AppendLine("{");

			// FUNCTION: OnEnable()
			builder.AppendLine();

			builder.AppendLine("\tvoid OnEnable()");
			builder.AppendLine("\t{");
			builder.AppendLine("\t}");
			// END FUNCTION

			// FUNCTION: OnInspectorGUI()
			builder.AppendLine();

			builder.AppendLine("\tpublic override void OnInspectorGUI()");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tDrawDefaultInspector();");
			builder.AppendLine("\t}");
			// END FUNCTION

			builder.AppendLine();
			builder.AppendLine("}");
			// END FUNCTION

			return builder.ToString();
		}

		static string UppercaseFirst(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToUpper(s[0]) + s.Substring(1);
		}
    }
}