using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using System.Runtime.CompilerServices;

namespace Nootools.Collections
{
    [Serializable]
    public struct BitSet8x8 : IEquatable<BitSet8x8>
    {
        private ulong data;

        public bool this[int2 index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return IsSet(index);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if(value)
                {
                    Set(index);
                }
                else
                {
                    Unset(index);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int2 index)
        {
            data |= Bit(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unset(int2 index)
        {
            data &= ~Bit(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSet(int2 index)
        {
            return (data & Bit(index)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAny()
        {
            return data != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAll()
        {
            return data == ulong.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNone()
        {
            return data == ulong.MinValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Bit(int2 index)
        {
            return 1UL << (index.x + index.y << 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Bit(int x, int y)
        {
            return 1UL << (x + y << 3);
        }

        public override bool Equals(object obj)
        {
            return obj is BitSet8x8 && Equals((BitSet8x8)obj);
        }

        public bool Equals(BitSet8x8 other)
        {
            return data == other.data;
        }

        public override int GetHashCode() => data.GetHashCode();
    }
}
