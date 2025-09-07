using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public class TableView
    {
        public readonly float HEADER_HEIGHT = 28f;
        public readonly float ROW_HEIGHT = 22f;

        readonly SearchField searchField;
        string searchText;

        readonly MultiColumnHeader header;
        readonly MultiColumnHeaderState headerState;

        readonly ITable data;

        Vector2 scrollPos;

        Rect headerRect;
        Rect tableRect;
        Rect tableViewRect;

        List<TableModel> rows;
        List<TableModel> filteredRows;

        readonly List<TableColumn> columns;

        readonly Texture2D rowOddTexture;
        readonly Texture2D rowEvenTexture;
        TableExpressionParser.BoolExp rowColorCondition;

        string SID { get { return "TableView" + data.GetObjectType().Name; } }

        MultiColumnHeaderState GetDefaultHeaderState()
        {
            return new MultiColumnHeaderState
            (
                columns
                .Where(x => x.attribute == null || x.attribute.visible == true)
                .Select(x => new MultiColumnHeaderState.Column
                {
                    canSort = x.Sortable,
                    headerContent = new GUIContent(x.DisplayName),
                    width = x.width > 0f ? x.width : -1f,
                    autoResize = x.width <= 0f,
                    maxWidth = x.width > 0f ? x.width : 1000f,
                    minWidth = x.width > 0f ? x.width : 20f,
                }
                ).ToArray()
            );
        }

        public TableView(ITable data)
        {
            this.data = data;
            columns = data.GetColumns();

            searchField = new SearchField();

            headerState = GetDefaultHeaderState();

            string prevHeaderStateContent = SessionState.GetString(SID + "Header", string.Empty);
            if (!string.IsNullOrEmpty(prevHeaderStateContent))
            {
                MultiColumnHeaderState prevHeaderState = GetDefaultHeaderState();
                EditorJsonUtility.FromJsonOverwrite(prevHeaderStateContent, prevHeaderState);

                if (MultiColumnHeaderState.CanOverwriteSerializedFields(prevHeaderState, headerState))
                {
                    MultiColumnHeaderState.OverwriteSerializedFields(prevHeaderState, headerState);
                }
            }

            string prevSearchText = SessionState.GetString(SID + "Search", string.Empty);
            if (!string.IsNullOrEmpty(prevSearchText)) { searchText = prevSearchText; }

            header = new MultiColumnHeader(headerState)
            {
                height = HEADER_HEIGHT
            };

            header.ResizeToFit();

            if (EditorGUIUtility.isProSkin)
            {
                rowEvenTexture = new Texture2D(1, 1);
                rowEvenTexture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f));
                rowEvenTexture.Apply();

                rowOddTexture = new Texture2D(1, 1);
                rowOddTexture.SetPixel(0, 0, new Color(0.17f, 0.17f, 0.17f));
                rowOddTexture.Apply();
            }
            else
            {
                rowEvenTexture = new Texture2D(1, 1);
                rowEvenTexture.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.7f));
                rowEvenTexture.Apply();

                rowOddTexture = new Texture2D(1, 1);
                rowOddTexture.SetPixel(0, 0, new Color(0.7f, 0.7f, 0.7f));
                rowOddTexture.Apply();
            }

            header.sortingChanged += Header_sortingChanged;

            UpdateRows();
        }

        public void UpdateRows()
        {
            rows = data.GetRows().ToList();
            FilterRows(searchText);
        }

        public void FilterRows(string searchText)
        {
            try
            {
                var searchBoolExp = TableExpressionParser.Compile(data.GetRowType(), searchText);
                FilterRowsByExp(searchBoolExp);
            }
            catch
            {
                FilterRowsByString(searchText);
            }
        }

        public void FilterRowsByExp(TableExpressionParser.BoolExp exp)
        {
            filteredRows = rows.Where(x => exp.Eval(x)).ToList();
            Header_sortingChanged(header);
        }

        public void FilterRowsByString(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filteredRows = rows;
            }
            else
            {
                filteredRows = rows.Where((x) =>
                {
                    if (x.model == null) return false;

                    foreach (var col in columns)
                    {
                        var value = col.property.GetValue(x, null);

                        var textValue = string.Empty;

                        if (col.type == typeof(string))
                        {
                            textValue = (string)value;
                        }
                        else if (col.type == typeof(float))
                        {
                            textValue = value.ToString();
                        }
                        else if (col.type == typeof(int))
                        {
                            textValue = value.ToString();
                        }
                        else if (typeof(UnityEngine.Object).IsAssignableFrom(col.type))
                        {
                            if ((UnityEngine.Object)value != null) textValue = ((UnityEngine.Object)value).name;
                        }

                        if (!string.IsNullOrEmpty(textValue) && textValue.ToLower().Contains(filter.ToLower())) return true;
                    }

                    return false;
                }).ToList();
            }

            Header_sortingChanged(header);
        }

        private void Header_sortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SessionState.SetString(SID + "Header", EditorJsonUtility.ToJson(headerState));

            if (multiColumnHeader.sortedColumnIndex < 0) return;

            var col = columns[multiColumnHeader.sortedColumnIndex];
            var headerColumn = headerState.columns[multiColumnHeader.sortedColumnIndex];
            var sortMod = headerColumn.sortedAscending ? 1 : -1;
            var type = col.SortingProperty.PropertyType;

            filteredRows.Sort((a, b) =>
            {
                var valA = col.SortingProperty.GetValue(a, null);
                var valB = col.SortingProperty.GetValue(b, null);

                if (type == typeof(string))
                {
                    return ((string)valA).CompareTo(valB) * sortMod;
                }
                else if (type == typeof(float))
                {
                    return ((float)valA).CompareTo(valB) * sortMod;
                }
                else if (type == typeof(int))
                {
                    return ((int)valA).CompareTo(valB) * sortMod;
                }
                else if (type == typeof(bool))
                {
                    return ((bool)valA).CompareTo(valB) * sortMod;
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    if (valA == null && valB == null) return 0;
                    if (valA == null) return sortMod * 1;
                    if (valB == null) return sortMod * -1;

                    return ((UnityEngine.Object)valA).name.CompareTo(((UnityEngine.Object)valB).name) * sortMod;
                }

                return 0;
            });
        }

        public void OnGUI(Rect rect)
        {
            // Toolbar
            var toolbarRect = rect;
            toolbarRect.height = 18f;

            EditorGUI.BeginChangeCheck();
            searchText = searchField.OnGUI(toolbarRect, searchText);
            if (EditorGUI.EndChangeCheck())
            {
                SessionState.SetString(SID + "Search", searchText);
                FilterRows(searchText);
            }

            // Header
            headerRect = rect;
            headerRect.y += toolbarRect.height;
            headerRect.height = HEADER_HEIGHT;

            var rowIndex = 0;
            var rowHeight = ROW_HEIGHT;

            header.OnGUI(headerRect, scrollPos.x);

            tableRect = rect;
            tableRect.y += headerRect.height + toolbarRect.height;
            tableRect.height -= headerRect.height + 18f + toolbarRect.height;

            tableViewRect = tableRect;

            tableRect.height = rowHeight * filteredRows.Count();
            tableRect.width = headerState.widthOfAllVisibleColumns;

            scrollPos = GUI.BeginScrollView(tableViewRect, scrollPos, tableRect);

            var rowType = data.GetRowType();

            RowAttribute rowAttribute = rowType.GetCustomAttributes(typeof(RowAttribute), true).FirstOrDefault() as RowAttribute;
            if (rowAttribute != null && !string.IsNullOrEmpty(rowAttribute.colorIf) && rowColorCondition == null)
            {
                try
                {
                    rowColorCondition = TableExpressionParser.Compile(rowType, rowAttribute.colorIf);
                }
                catch
                {
                    throw new Exception(string.Format("Invalid expression for row ({0}) on attribute (colorIf).", rowType.Name));
                }
            }

            foreach (var row in filteredRows)
            {
                var oldRowGUIColor = GUI.color;

                if (rowAttribute != null)
                {
                    GUI.color = (rowColorCondition == null || rowColorCondition.Eval(row)) ? rowAttribute.Color : Color.white;
                }

                if ((rowIndex % 2 == 0))
                {
                    GUI.color = Color.Lerp(GUI.color, Color.black, 0.1f);
                }

                var rowRect = new Rect(0f, rowIndex * rowHeight + tableRect.y, headerState.widthOfAllVisibleColumns, rowHeight);
                var colRect = rowRect;
                colRect.y += 2f;
                colRect.height -= 4f;

                colRect.width = 0f;

                GUI.DrawTexture(rowRect, (rowIndex % 2 == 0) ? rowEvenTexture : rowOddTexture);

                rowIndex++;

                if (row.model == null)
                {
                    continue;
                }

                for (int i = 0; i < headerState.visibleColumns.Length; i++)
                {
                    var colIndex = headerState.visibleColumns[i];

                    var col = columns[colIndex];
                    var headerColumn = headerState.columns[colIndex];

                    colRect.x += colRect.width + 4f;
                    colRect.width = headerColumn.width - 8f;

                    var val = col.GetValue(row);

                    if (colIndex == 0)
                    {
                        var selectBtnRect = colRect;
                        selectBtnRect.y -= 1;
                        if (GUI.Button(selectBtnRect, "Select", EditorStyles.toolbarButton))
                        {
                            EditorGUIUtility.PingObject(row.model);
                            Selection.activeObject = row.model;
                        }
                    }
                    else
                    {
                        var oldGUIColor = GUI.color;

                        if (col.attribute != null)
                        {
                            GUI.color = col.GetColor(row);
                        }

                        if ((rowIndex % 2 == 1))
                        {
                            GUI.color = Color.Lerp(GUI.color, Color.black, 0.1f);
                        }

                        EditorGUI.BeginDisabledGroup(!col.editable);

                        EditorGUI.BeginChangeCheck();

                        if (col.type == typeof(string))
                        {
                            val = EditorGUI.TextField(colRect, (string)val);
                        }
                        else if (col.type == typeof(float))
                        {
                            val = EditorGUI.FloatField(colRect, (float)val);
                        }
                        else if (col.type == typeof(int))
                        {
                            val = EditorGUI.IntField(colRect, (int)val);
                        }
                        else if (col.type == typeof(bool))
                        {
                            val = EditorGUI.Toggle(colRect, (bool)val);
                        }
                        else if (col.type == typeof(Sfloat))
                        {
                            val = SfPropertyDrawer.SfloatField(colRect, (Sfloat)val);
                        }
                        else if (col.editable == false && col.type == typeof(Sprite))
                        {
                            var sprite = (Sprite)val;

                            if (sprite != null)
                            {
                                Vector2 fullSize = new(sprite.texture.width, sprite.texture.height);
                                Vector2 size = new(sprite.textureRect.width, sprite.textureRect.height);

                                var position = colRect;

                                Rect coords = sprite.textureRect;
                                coords.x /= fullSize.x;
                                coords.width /= fullSize.x;
                                coords.y /= fullSize.y;
                                coords.height /= fullSize.y;

                                Vector2 ratio;
                                ratio.x = position.width / size.x;
                                ratio.y = position.height / size.y;
                                float minRatio = Mathf.Min(ratio.x, ratio.y);

                                Vector2 center = position.center;
                                position.width = size.x * minRatio;
                                position.height = size.y * minRatio;
                                position.center = center;

                                GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
                            }
                        }
                        else if (typeof(UnityEngine.Object).IsAssignableFrom(col.type))
                        {
                            val = EditorGUI.ObjectField(colRect, (UnityEngine.Object)val, col.type, false);
                        }
                        else if (col.type.IsEnum)
                        {
                            if (col.type.GetCustomAttributes(typeof(FlagsAttribute), true).Length == 0)
                            {
                                val = EditorGUI.EnumPopup(colRect, (Enum)val);
                            }
                            else
                            {
                                val = EditorGUI.EnumFlagsField(colRect, (Enum)val);
                            }
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            col.SetValue(row, val);
                            EditorUtility.SetDirty(row.model);
                        }

                        EditorGUI.EndDisabledGroup();

                        GUI.color = oldGUIColor;
                    }

                    colRect.width += 4f;
                }

                GUI.color = oldRowGUIColor;
            }

            GUI.EndScrollView();
        }

        public void OnRowGUI()
        {


        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public float width;
        public int color = 0xffffff;
        public string colorIf;
        public bool visible = true;
        public string sortBy;

        public Color Color
        {
            get
            {
                Color32 c = new()
                {
                    b = (byte)((color) & 0xFF),
                    g = (byte)((color >> 8) & 0xFF),
                    r = (byte)((color >> 16) & 0xFF),
                    a = (byte)(0xFF)
                };
                return c;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RowAttribute : Attribute
    {
        public int color = 0xffffff;
        public string colorIf;

        public Color Color
        {
            get
            {
                Color32 c = new()
                {
                    b = (byte)((color) & 0xFF),
                    g = (byte)((color >> 8) & 0xFF),
                    r = (byte)((color >> 16) & 0xFF),
                    a = (byte)(0xFF)
                };
                return c;
            }
        }
    }
}