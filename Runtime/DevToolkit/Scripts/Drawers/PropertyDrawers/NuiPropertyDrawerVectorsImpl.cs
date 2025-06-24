using Unity.Mathematics;
using UnityEngine;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerVector2 : NuiPropertyDrawerFloatVectors<Vector2>
    {
        public NuiPropertyDrawerVector2() : base("X", "Y") { }

        protected override Vector2 VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
            }
        }
    }

    public class NuiPropertyDrawerVector3 : NuiPropertyDrawerFloatVectors<Vector3>
    {
        public NuiPropertyDrawerVector3() : base("X", "Y", "Z") { }

        protected override Vector3 VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"], fieldValues["Z"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
                fieldValues["Z"] = value.z;
            }
        }
    }

    public class NuiPropertyDrawerVector4 : NuiPropertyDrawerFloatVectors<Vector4>
    {
        public NuiPropertyDrawerVector4() : base("X", "Y", "Z", "W") { }

        protected override Vector4 VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"], fieldValues["Z"], fieldValues["W"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
                fieldValues["Z"] = value.z;
                fieldValues["W"] = value.w;
            }
        }
    }

    public class NuiPropertyDrawerVector2Int : NuiPropertyDrawerIntVectors<Vector2Int>
    {
        public NuiPropertyDrawerVector2Int() : base("X", "Y") { }

        protected override Vector2Int VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
            }
        }
    }

    public class NuiPropertyDrawerVector3Int : NuiPropertyDrawerIntVectors<Vector3Int>
    {
        public NuiPropertyDrawerVector3Int() : base("X", "Y", "Z") { }

        protected override Vector3Int VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"], fieldValues["Z"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
                fieldValues["Z"] = value.z;
            }
        }
    }

    public class NuiPropertyDrawerInt2 : NuiPropertyDrawerIntVectors<int2>
    {
        public NuiPropertyDrawerInt2() : base("X", "Y") { }

        protected override int2 VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
            }
        }
    }

    public class NuiPropertyDrawerInt3 : NuiPropertyDrawerIntVectors<int3>
    {
        public NuiPropertyDrawerInt3() : base("X", "Y", "Z") { }

        protected override int3 VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"], fieldValues["Z"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
                fieldValues["Z"] = value.z;
            }
        }
    }

    public class NuiPropertyDrawerInt4 : NuiPropertyDrawerIntVectors<int4>
    {
        public NuiPropertyDrawerInt4() : base("X", "Y", "Z", "W") { }

        protected override int4 VectorValue
        {
            get => new(fieldValues["X"], fieldValues["Y"], fieldValues["Z"], fieldValues["W"]);
            set
            {
                fieldValues["X"] = value.x;
                fieldValues["Y"] = value.y;
                fieldValues["Z"] = value.z;
                fieldValues["W"] = value.w;
            }
        }
    }
}
