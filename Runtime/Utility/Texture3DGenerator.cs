using Sirenix.OdinInspector;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript, CreateAssetMenu(menuName = "Noo.Tools/3d Texture Generator", fileName = "3d Texture", order = -10000)]
    public class Texture3DGenerator : ScriptableObject
    {
        public string assetName;
        public Texture2D[] sourceTextures;

        public int3 size;
        public TextureFormat format = TextureFormat.RGBA32;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

#if UNITY_EDITOR
        [Button]
        void Generate()
        {
            Texture3D texture = new(size.x, size.y, size.z, format, false)
            {
                wrapMode = wrapMode
            };

            Color[] colors = new Color[size.x * size.y * size.z];

            for (int z = 0; z < size.z; z++)
            {
                var sourceTex = sourceTextures[z];

                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        colors[x + y * size.x + z * size.x * size.y] = sourceTex.GetPixel(x, y);
                    }
                }
            }

            texture.SetPixels(colors);
            texture.Apply();

            var assetPath = AssetDatabase.GetAssetPath(this);

            AssetDatabase.CreateAsset(texture, $"{Path.GetDirectoryName(assetPath)}/{assetName}.asset");
            AssetDatabase.Refresh();
        }
#endif

    }
}
