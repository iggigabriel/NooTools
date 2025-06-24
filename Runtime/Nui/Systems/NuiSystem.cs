using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(UIDocument))]
    public abstract class NuiSystem : MonoBehaviour
    {
        static readonly Dictionary<IPanel, Dictionary<Type, NuiSystem>> instances = new();

        public UIDocument Document { get; private set; }
        public IPanel Panel { get; private set; }

        protected virtual void Awake()
        {
            Document = GetComponent<UIDocument>();
            if (Document) Panel = Document.runtimePanel;
        }

        protected virtual void OnEnable()
        {
            if (Panel != null)
            {
                if (!instances.TryGetValue(Panel, out var panelSystems))
                {
                    instances[Panel] = panelSystems = new Dictionary<Type, NuiSystem>();
                }

                panelSystems[GetType()] = this;
            }
        }

        protected virtual void OnDisable()
        {

        }

        public static bool TryFind<T>(IPanel panel, out T system) where T : NuiSystem
        {
            if (panel == null)
            {
                system = default;
                return false;
            }

            if (instances.TryGetValue(panel, out var panelSystems))
            {
                if (panelSystems.TryGetValue(typeof(T), out var panelSystem) && panelSystem is T tSystem)
                {
                    system = tSystem;
                    return true;
                }
            }

            system = default;
            return false;
        }
    }
}
