using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public class TableWindow : EditorWindow
    {
        [MenuItem("Tools/Tables", priority = 10000)]
        static void Init()
        {
            var window = (TableWindow)EditorWindow.GetWindow(typeof(TableWindow));
            window.titleContent = new GUIContent("Tables");
            window.Show();
        }

        [NonSerialized] TableView tableView;

        [NonSerialized] string[] tableNames;
        [NonSerialized] List<ITable> tableTypes;

        [NonSerialized] ITable tableData;

        int SelectedTable { get { return EditorPrefs.GetInt("TableWindow_selectedTable", 0); } set { EditorPrefs.SetInt("TableWindow_selectedTable", value); } }

        private void OnEnable()
        {
            tableTypes = new List<ITable>();

            foreach (Type type in
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsAbstract && p.IsPublic && p.IsClass && typeof(ITable).IsAssignableFrom(p))
            )
            {
                tableTypes.Add(Activator.CreateInstance(type) as ITable);
            }

            tableNames = tableTypes.Select(x => x.GetType().Name).ToArray();

            OnSelectedTableChanged();
        }

        void OnSelectedTableChanged()
        {
            var id = SelectedTable;

            if (id < 0 || id >= tableTypes.Count)
            {
                tableView = null;
                tableData = null;
            }
            else
            {
                tableData = tableTypes[id];
                tableView = new TableView(tableData);
            }
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUI.BeginChangeCheck();
            SelectedTable = EditorGUILayout.Popup(SelectedTable, tableNames, EditorStyles.toolbarDropDown);
            if (EditorGUI.EndChangeCheck())
            {
                OnSelectedTableChanged();
            }

            if (GUILayout.Button("Refresh Data", EditorStyles.toolbarButton))
            {
                tableView?.UpdateRows();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            var toolbarRect = GUILayoutUtility.GetLastRect();

            var tableRect = position;
            tableRect.y = toolbarRect.height;
            tableRect.x = 0f;

            tableView?.OnGUI(tableRect);
        }

    }

}
