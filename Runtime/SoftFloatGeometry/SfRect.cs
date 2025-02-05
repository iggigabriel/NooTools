using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Noo.Tools
{
    [Serializable]
    public struct SfRect : ISfShape
    {
        public Sfloat2 min;
        public Sfloat2 size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfRect(Sfloat2 min, Sfloat2 size)
        {
            this.min = min;
            this.size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfRect(Sfloat x, Sfloat y, Sfloat width, Sfloat height)
        {
            min = new Sfloat2(x, y);
            size = new Sfloat2(width, height);
        }

        public Sfloat2 Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => min + size;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => size = value - min;
        }

        public Sfloat Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => size.x;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => size.x = value;
        }

        public Sfloat Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => size.y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => size.y = value;
        }

        public Sfloat2 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => min + (size >> 1);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => min = value - (size >> 1);
        }

        public readonly SfRect Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect MinMaxRect(Sfloat2 min, Sfloat2 max)
        {
            return new SfRect(min, max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect CenterRadius(Sfloat2 center, Sfloat2 radius)
        {
            return new SfRect(center - radius, radius + radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect FromPoints(Sfloat2 p1, Sfloat2 p2)
        {
            return MinMaxRect(Sfloat2.Min(p1, p2), Sfloat2.Max(p1, p2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(Sfloat2 point)
        {
            var max = Max;
            return point.x.Raw >= min.x.Raw && point.x.Raw < max.x.Raw && point.y.Raw >= min.y.Raw && point.y.Raw < max.y.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect Offset(SfRect rect, Sfloat2 position) => new(rect.min + position, rect.size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect Expand(SfRect rect, Sfloat radius) => new(rect.min - radius, rect.size + (radius << 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect Expand(SfRect rect, Sfloat2 radius) => new(rect.min - radius, rect.size + (radius << 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect MinSize(SfRect rect, Sfloat2 size) => new(rect.min, Sfloat2.Max(rect.size, size));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SfRect MaxSize(SfRect rect, Sfloat2 size) => new(rect.min, Sfloat2.Min(rect.size, size));

        public static SfRect ClampSize(SfRect rect, Sfloat2 sizeMin, Sfloat2 sizeMax) => new(rect.min, Sfloat2.Clamp(rect.size, sizeMin, sizeMax));

        public static SfRect Join(SfRect a, SfRect b) => MinMaxRect(Sfloat2.Min(a.min, b.min), Sfloat2.Max(a.Max, b.Max));

        public static SfRect JoinMany<TArray, TShape>(TArray shapes) where TArray : IReadOnlyList<TShape> where TShape : ISfShape
        {
            if (shapes.Count == 0) return default;

            var bounds = shapes[0].Bounds;

            var min = bounds.min;
            var max = bounds.Max;

            for (int i = 1; i < shapes.Count; i++)
            {
                bounds = shapes[i].Bounds;
                min = Sfloat2.Min(min, bounds.min);
                max = Sfloat2.Max(max, bounds.Max);
            }

            return MinMaxRect(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Overlaps(SfRect other)
        {
            var max = Max;
            var otherMax = other.Max;
            return otherMax.x > min.x && other.min.x < max.x && otherMax.y > min.y && other.min.y < max.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Overlaps(SfCircle circle) => SfGeom.Overlaps(circle, this);

        public override readonly string ToString()
        {
            return $"Rect(min: {min.ToString("0.000")}, size: {size.ToString("0.000")}";
        }

        public readonly SfCapsule GetInnerCapsule()
        {
            var halfSize = size >> 1;

            if (size.x > size.y)
            {
                var p1 = min + halfSize.y;
                return new SfCapsule(p1, new Sfloat2(p1.x + size.x - size.y, p1.y), halfSize.y);
            }
            else
            {
                var p1 = min + halfSize.x;
                return new SfCapsule(p1, new Sfloat2(p1.x, p1.y + size.y - size.x), halfSize.x);
            }
        }
    }
}
