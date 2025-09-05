using Noo.Nui;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Noo.DevToolkit
{
    public class NuiMethodDrawer : NuiPropertyDrawer<MethodInfo>
    {
        MethodInfo methodInfo;
        ParameterInfo[] parameters;
        object[] parameterDefaultValues;
        object[] parameterValues;
        NuiPropertyDrawer[] parameterDrawers;

        NuiButton btn;

        public override void OnInit()
        {
            base.OnInit();

            methodInfo = Value;
            parameters = methodInfo.GetParameters();
            parameterDefaultValues = new object[parameters.Length];
            parameterValues = new object[parameters.Length];
            parameterDrawers = new NuiPropertyDrawer[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                parameterDrawers[i] = NuiDrawerUtility.CreateDrawer(parameter);
                parameterDefaultValues[i] = parameterDrawers[i].Property.Value;
                AddDrawer(parameterDrawers[i]);
            }
        }

        protected override void OnPropertyCreate()
        {
            Root.WithClass("dtk-method-drawer", "nui-panel");

            if (parameters.Length == 0) Root.WithClass("dtk-method-drawer__noparams");

            btn = NuiPool.Rent<NuiButton>();
            btn.WithClass("nui-btn-light-black");
            btn.ButtonText = string.IsNullOrEmpty(Info) ? "Run" : Info;
            btn.IconRight = MatIcon.PlayArrow;
            btn.clicked += OnInvoke;
            LabelContainer.Add(btn);
        }

        protected override void OnPropertyDestroy()
        {
            Root.WithoutClass("dtk-method-drawer", "nui-panel").WithoutClass("dtk-method-drawer__noparams");

            if (btn != null)
            {
                btn.RemoveFromHierarchy();
                btn.WithoutClass("nui-btn-light-black");
                btn.clicked -= OnInvoke;
                NuiPool.Return(btn);
                btn = null;
            }
        }

        protected override bool OnPropertyFilter(string query)
        {
            return methodInfo.Name.ToLowerInvariant().Contains(query);
        }

        static readonly Stopwatch sw = Stopwatch.StartNew();

        void OnInvoke()
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = parameterDrawers[i].Property.Value;
            }

            if (Property.HasValidTarget(out var target))
            {
                var elapsedMs = sw.Elapsed.TotalMilliseconds;

                try
                {
                    methodInfo.Invoke(target, parameterValues);
                    elapsedMs = sw.Elapsed.TotalMilliseconds - elapsedMs;
                    UnityEngine.Debug.Log($"[DevConsole] {methodInfo.DeclaringType.Name}.{methodInfo.Name}({string.Join(", ", parameterValues)}) \u23F1\uFE0F{elapsedMs:0.00}ms");
                }
                catch (Exception e)
                {
                    elapsedMs = sw.Elapsed.TotalMilliseconds - elapsedMs;
                    UnityEngine.Debug.Log($"[DevConsole] {methodInfo.DeclaringType.Name}.{methodInfo.Name}({string.Join(", ", parameterValues)}) ️\u23F1\uFE0F{elapsedMs:0.00}ms");
                    UnityEngine.Debug.LogException(e);
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"No valid instance of type ({methodInfo.DeclaringType.Name}) found to invoke method ({methodInfo.Name}).");
            }
        }
    }
}
