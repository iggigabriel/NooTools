using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public struct ChildrenEnumerator : IEnumerator<VisualElement>, IEnumerator, IDisposable
    {
        readonly VisualElement parent;

        VisualElement current;

        public ChildrenEnumerator(VisualElement parent)
        {
            this.parent = parent;
            current = parent;
        }

        public readonly VisualElement Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return current;
            }
        }

        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Current;
            }
        }

        public readonly void Dispose()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (current != null) 
            {
                if (current.childCount > 0)
                {
                    current = current[0];
                }
                else
                {
                    while (current != parent)
                    {
                        var parent = current.parent;

                        if (parent == null)
                        {
                            current = null;
                        }
                        else
                        {
                            var index = parent.IndexOf(current) + 1;

                            if (index <= 0)
                            {
                                current = null;
                                break;
                            }
                            else if (index >= parent.childCount)
                            {
                                current = parent;
                            }
                            else
                            {
                                current = parent[index];
                                break;
                            }
                        }
                    }

                    if (current == parent) current = null;
                }
            }

            return current != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            current = parent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ChildrenEnumerator GetEnumerator()
        {
            return this;
        }
    }
}
