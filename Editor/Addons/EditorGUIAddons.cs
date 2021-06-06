using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Nootools.Editor
{
	public class EditorGUIAddons
	{
		public static void DrawRectOutline(Rect rect, int outlineWidth, Color rectColor, Color outlineColor)
		{
			EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, outlineWidth), outlineColor);
			EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - outlineWidth, rect.width, outlineWidth), outlineColor);

			EditorGUI.DrawRect(new Rect(rect.x, rect.y + outlineWidth, outlineWidth, rect.height - outlineWidth * 2), outlineColor);
			EditorGUI.DrawRect(new Rect(rect.x + rect.width - outlineWidth, rect.y + outlineWidth, outlineWidth, rect.height - outlineWidth * 2), outlineColor);

			EditorGUI.DrawRect(new Rect(rect.x + outlineWidth, rect.y + outlineWidth, rect.width - outlineWidth * 2, rect.height - outlineWidth * 2), rectColor);
		}


		/// <summary>
		/// Creates an array foldout like in inspectors for SerializedProperty of array type.
		/// Counterpart for standard EditorGUILayout.PropertyField which doesn't support SerializedProperty of array type.
		/// </summary>
		public static void ArrayField (SerializedProperty property)
		{
			bool wasEnabled = GUI.enabled;
			int prevIdentLevel = EditorGUI.indentLevel;
			
        // Iterate over all child properties of array
			bool childrenAreExpanded = true;
			int propertyStartingDepth = property.depth;
			while (property.NextVisible(childrenAreExpanded) && propertyStartingDepth < property.depth)
			{
				childrenAreExpanded = EditorGUILayout.PropertyField(property);
			}
			
			EditorGUI.indentLevel = prevIdentLevel;
			GUI.enabled = wasEnabled;
		}
		
    /// <summary>
    /// Creates a filepath textfield with a browse button. Opens the open file panel.
    /// </summary>
		public static string FileLabel(string name, string path, string extension)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(name);
			string filepath = EditorGUILayout.TextField(path);
			if (GUILayout.Button("Browse"))
			{
				filepath = EditorUtility.OpenFilePanel(name, path, extension);
			}
			EditorGUILayout.EndHorizontal();
			return filepath;
		}
		
    /// <summary>
    /// Creates a folder path textfield with a browse button. Opens the save folder panel.
    /// </summary>
		public static string FolderLabel(string name, string path)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(name);
			string filepath = EditorGUILayout.TextField(name, path);
			if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
			{
				filepath = EditorUtility.SaveFolderPanel(name, path, "Folder");
			}
			EditorGUILayout.EndHorizontal();
			return filepath;
		}
		
    /// <summary>
    /// Creates an array foldout like in inspectors. Hand editable ftw!
    /// </summary>
		public static string[] ArrayFoldout(string label, string[] array, ref bool foldout)
		{
			EditorGUILayout.BeginVertical();
			foldout = EditorGUILayout.Foldout(foldout, label);
			string[] newArray = array;
			if (foldout)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical();
				int arraySize = EditorGUILayout.IntField("Size", array.Length);
				if (arraySize != array.Length)
					newArray = new string[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					string entry = "";
					if (i < array.Length)
						entry = array[i];
					newArray[i] = EditorGUILayout.TextField("Element " + i, entry);
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			return newArray;
		}
		
		/// <summary>
		/// Creates a toolbar that is filled in from an Enum. Useful for setting tool modes.
		/// </summary>
		public static Enum EnumToolbar(Enum selected, string[] suffixes = null)
		{
			string[] toolbar = System.Enum.GetNames(selected.GetType());
			Array values = System.Enum.GetValues(selected.GetType());
			
			for (int i=0; i  < toolbar.Length; i++)
			{
				string toolname = toolbar[i];
				toolname = toolname.Replace("_", " ");
				if (suffixes != null && suffixes.Length > i) toolname += string.Format("{0}", suffixes[i]);
				toolbar[i] = toolname;
			}
			
			int selected_index = 0;
			while (selected_index < values.Length)
			{
				if (selected.ToString() == values.GetValue(selected_index).ToString())
				{
					break;
				}
				selected_index++;
			}
			selected_index = GUILayout.Toolbar(selected_index, toolbar);
			return (Enum) values.GetValue(selected_index);
		}

		public static bool EnumMenu<T>(out T clicked) where T : Enum
		{
			var enumType = typeof(T);
			string[] toolbar = Enum.GetNames(enumType);
			Array values = Enum.GetValues(enumType);

			using var _ = new EditorGUILayout.HorizontalScope();
			var isClicked = false;
			clicked = default;

			for (int i = 0; i < toolbar.Length; i++)
            {
				if (GUILayout.Button(toolbar[i]))
                {
					isClicked = true;
					clicked = (T)values.GetValue(i);
                }
            }

			return isClicked;
		}

        /// <summary>
        /// Creates a button that can be toggled. Looks nice than GUI.toggle
        /// </summary>
        /// <returns>
        /// Toggle state
        /// </returns>
        /// <param name='state'>
        /// If set to <c>true</c> state.
        /// </param>
        /// <param name='label'>
        /// If set to <c>true</c> label.
        /// </param>
        public static bool ToggleButton(bool state, string label)
		{
			BuildStyle();
			
			bool out_bool = false;
			
			if (state)
				out_bool = GUILayout.Button(label, toggled_style);
			else
				out_bool = GUILayout.Button(label);
			
			if (out_bool)
				return !state;
			else
				return state;
		}
		
		public class ModalPopupWindow : EditorWindow
		{
			public event System.Action<bool> OnChosen;
			string popText = "";
			string trueText = "Yes";
			string falseText = "No";
			
			public void SetValue(string text, string accept, string no)
			{
				this.popText = text;
				this.trueText = accept;
				this.falseText = no;
			}
			
			void OnGUI()
			{
				GUILayout.BeginVertical();
				GUILayout.Label(popText);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(trueText))
				{
					if (OnChosen != null)
						OnChosen(true);
					this.Close();
				}
				if (GUILayout.Button(falseText))
				{
					if (OnChosen != null)
						OnChosen(false);
					this.Close();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}
		
		
		
		static GUIStyle toggled_style;
		public GUIStyle StyleButtonToggled
		{
			get
			{
				BuildStyle();
				return toggled_style;
			}
		}
		
		static GUIStyle labelText_style;
		public static GUIStyle StyleLabelText
		{
			get
			{
				BuildStyle();
				return labelText_style;
			}
		}
		
		private static void BuildStyle() {
			if (toggled_style == null)
			{
				toggled_style = new GUIStyle(GUI.skin.button);
				toggled_style.normal.background = toggled_style.onActive.background;
				toggled_style.normal.textColor = toggled_style.onActive.textColor;
			}
			if (labelText_style == null)
			{
				labelText_style = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textField);
				labelText_style.normal = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button.onNormal;
			}
		}



		public static SerializedObject GetSerializedComponent<T> (SerializedObject source) where T : Component
		{
			UnityEngine.Object[] targets = source.targetObjects.Select ((UnityEngine.Object arg) => (arg as Component).GetComponent <T> ()).ToArray ();
			return new SerializedObject (targets);
		}
	}
}

