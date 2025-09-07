using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    /// <summary>
    /// Class needs to be public to be visible in global Tables editor
    /// </summary>
    internal class ExampleTable : Table<ScriptableObject, ExampleTableModel>
    {
        public override IEnumerable<ScriptableObject> GetObjects()
        {
            return
                AssetDatabase.FindAssets("t:ScriptableObject")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<ScriptableObject>(x))
                .Where(x => x);
        }
    }

    internal class ExampleTableModel : TableModel<ScriptableObject>
    {
        [Column(width = 220)]
        public string Name
        {
            get
            {
                return Target.name;
            }
            set
            {
                Target.name = value;
            }
        }
    }

}
