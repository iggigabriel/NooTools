using UnityEngine;

namespace Noo.Tools
{
    public static class TextureExtensions
    {
        public static float AspectRatio(this Texture texture)
        {
            return texture.width / (float)texture.height;
        }
    }
}
