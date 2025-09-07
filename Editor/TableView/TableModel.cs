using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public interface ITable
    {
        IEnumerable<TableModel> GetRows();
        List<TableColumn> GetColumns();

        Type GetObjectType();
        Type GetRowType();
    }

    public abstract class Table<TObject, TModel> : ITable where TObject : UnityEngine.Object where TModel : TableModel<TObject>
    {
        public abstract IEnumerable<TObject> GetObjects();

        public IEnumerable<TableModel> GetRows()
        {
            return GetObjects()
                .Distinct()
                .Select(x => { var obj = Activator.CreateInstance<TModel>(); obj.model = x; return obj; })
                .Cast<TableModel>();
        }

        public Type GetObjectType()
        {
            return typeof(TObject);
        }

        public Type GetRowType()
        {
            return typeof(TModel);
        }

        public List<TableColumn> GetColumns()
        {
            var modelType = GetRowType();

            List<TableColumn> columns = new();

            var targetProp = modelType.GetProperty("Target", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic);

            columns.Add(new TableColumn
            {
                table = this,
                property = targetProp,
                name = "Object",
                type = targetProp.PropertyType,
                editable = false,
                width = 50f,
            });


            var props = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);

                ColumnAttribute attr = attrs.Select(x => x as ColumnAttribute).Where(x => x != null).FirstOrDefault();

                if (attr != null && !attr.visible) continue;

                columns.Add(new TableColumn
                {
                    table = this,
                    property = prop,
                    name = prop.Name,
                    type = prop.PropertyType,
                    editable = prop.CanWrite,
                    width = attr == null ? 0 : attr.width,
                    attribute = attr,
                });
            }

            return columns;
        }
    }

    public class TableColumn
    {
        public ITable table;
        public string name;
        public Type type;
        public bool editable;
        public PropertyInfo property;
        public float width;
        public ColumnAttribute attribute;

        PropertyInfo m_sortingProperty;
        TableExpressionParser.BoolExp m_colorCondition;

        public string DisplayName
        {
            get
            {
                name = name.First().ToString().ToUpper() + name.Substring(1);
                return Regex.Replace(name, "(?!^)([A-Z])", " $1");
            }
        }

        public bool Sortable
        {
            get
            {
                return isSortable(SortingProperty.PropertyType);
            }
        }

        public PropertyInfo SortingProperty
        {
            get
            {
                if (attribute != null && !string.IsNullOrEmpty(attribute.sortBy))
                {
                    AssertSortingProperty();
                    return m_sortingProperty;
                }
                else
                {
                    return property;
                }
            }
        }

        public Color GetColor(object instance)
        {
            if (attribute == null)
            {
                return Color.white;
            }

            if (!string.IsNullOrEmpty(attribute.colorIf))
            {
                if (m_colorCondition == null)
                {
                    try
                    {
                        m_colorCondition = TableExpressionParser.Compile(table.GetRowType(), attribute.colorIf);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Invalid expression in column ({0}) on attribute (colorIf).", name));
                    }
                }

                return m_colorCondition.Eval(instance) ? attribute.Color : Color.white;
            }

            return attribute.Color;
        }

        public object GetValue(object instance)
        {
            return property.GetValue(instance, null);
        }

        public void SetValue(object instance, object value)
        {
            property.SetValue(instance, value, null);
        }

        void AssertSortingProperty()
        {
            if (m_sortingProperty == null)
            {
                var parentType = table.GetRowType();
                m_sortingProperty = parentType.GetProperty(attribute.sortBy, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                if (m_sortingProperty == null)
                {
                    throw new Exception(string.Format("Column ({0}) sorting property ({1}) does not exist.", name, attribute.sortBy));
                }
            }
        }

        static readonly Type[] sortableTypes = new Type[]
        {
        typeof(string),
        typeof(int),
        typeof(bool),
        typeof(float),
        typeof(UnityEngine.Object)
        };

        static bool isSortable(Type type)
        {
            foreach (var sortableType in sortableTypes)
            {
                if (type == sortableType || sortableType.IsAssignableFrom(type)) return true;
            }

            return false;
        }

    }

    public abstract class TableModel
    {
        public UnityEngine.Object model;
    }

    public abstract class TableModel<T> : TableModel where T : UnityEngine.Object
    {
        protected T Target { get { return (T)model; } }
    }
}
