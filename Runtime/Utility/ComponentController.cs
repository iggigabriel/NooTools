using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript]
    public abstract class ComponentController<T> : MonoBehaviour where T : Component
    {
        [field: SerializeField, HideInInspector] 
        public T Target { get; private set; }

#pragma warning disable IDE0051 // Remove unused private members
        [ShowInInspector, PropertySpace(SpaceBefore = 0, SpaceAfter = 8), DisplayAsString, InlineButton("InjectTarget", SdfIconType.ArrowRepeat, ""), LabelText("Target", Icon = SdfIconType.Box), PropertyOrder(-100000f)]
        private string InspectorTarget { get { return Target ? Target.gameObject.name : "(None)"; } set { } }
#pragma warning restore IDE0051 // Remove unused private members

        private void InjectTarget() => Target = GetComponent<T>();

        protected virtual void Reset()
        {
            InjectTarget();
        }
    }
}
