using System;
using UnityEngine;

namespace Noo.Tools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RefType : PropertyAttribute
    {
        public RefType()
        {
        }
    }
}
