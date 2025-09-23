using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class DevConsole : MonoBehaviour
    {
        [SerializeField]
        UIDocument uiDocument;

        [SerializeField]
        int width = 320;

        [SerializeField]
        int height = 480;

        DtkWindow dtkWindow;

        private void Initialize()
        {
            if (dtkWindow != null) return;

            dtkWindow = new();
            dtkWindow.style.width = width;
            dtkWindow.style.height = height;

            var page = DevToolkit.Commands.rootVisualElement;

            dtkWindow.Add(page);
            dtkWindow.Title = page.Title;

            dtkWindow.IsDraggable = uiDocument.rootVisualElement.ClassListContains("nui--landscape");

            uiDocument.rootVisualElement.RegisterCallback<ScreenSizeChangeEvent>((e) =>
            {
                dtkWindow.IsDraggable = e.isLandscape;
            });

            dtkWindow.closeButton.clicked += () =>
            {
                gameObject.SetActive(false);
            };

            dtkWindow.appsButton.clicked += () => DevToolkit.Commands.ShowPage("");
        }

        private void OnEnable()
        {
            Initialize();
            uiDocument.rootVisualElement.Add(dtkWindow);
            DevToolkit.Commands.rootVisualElement.OnEnable();
            DevToolkit.Commands.ShowPage("");
        }

        private void OnDisable()
        {
            Initialize();
            DevToolkit.Commands.rootVisualElement.OnDisable();
            dtkWindow.RemoveFromHierarchy();
        }

        public static void ProcessHotkeys()
        {
            foreach (var (hotkey, command) in DevToolkitCommands.hotkeyInfos)
            {
                if (!hotkey.IsPressed) continue;

                try
                {
                    hotkey.Execute(command.memberInfo);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
