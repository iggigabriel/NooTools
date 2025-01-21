using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Noo.Tools
{
    public static class RenderingUtils
    {
        static readonly ObjectPool<MaterialPropertyBlock> mpbPool = new(() => new());
        
        public static MaterialPropertyBlock GetNewMaterialPropertyBlock() => mpbPool.Get();

        public static void ReleaseMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock)
        {
            if (materialPropertyBlock == null) return;
            materialPropertyBlock.Clear();
            mpbPool.Release(materialPropertyBlock);
        }

        static readonly Dictionary<string, int> shaderIdCache = new();

        public static int GetShaderId(string name)
        {
            if (!shaderIdCache.TryGetValue(name, out var id))
            {
                id = Shader.PropertyToID(name);
                shaderIdCache.Add(name, id);
            }

            return id;
        }
    }
}
