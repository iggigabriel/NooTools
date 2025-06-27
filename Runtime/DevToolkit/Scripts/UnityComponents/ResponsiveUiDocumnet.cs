using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    [RequireComponent(typeof(UIDocument))]
    public class ResponsiveUiDocumnet : MonoBehaviour
    {
        [SerializeField]
        UIDocument targetDocument;

        [SerializeField]
        float targetPortraitWidth = 360;

        [SerializeField]
        float targetLandscapeHeight = 768;

        Vector2Int? screenSize;

        private void Reset()
        {
            targetDocument = GetComponent<UIDocument>();
        }

        private void Awake()
        {
            if (targetDocument && targetDocument.panelSettings)
            {
                targetDocument.panelSettings = Instantiate(targetDocument.panelSettings);
            }
        }

        private void OnEnable()
        {
            OnScreenSizeChanged();
            screenSize = null;
        }

        private void LateUpdate()
        {
            if (!screenSize.HasValue || screenSize.Value != new Vector2Int(Screen.width, Screen.height))
            {
                OnScreenSizeChanged();
            }
        }

        private void OnScreenSizeChanged()
        {
            screenSize = new Vector2Int(Screen.width, Screen.height);

            if (!targetDocument) return;

            var isLandscape = screenSize.Value.x > screenSize.Value.y;

            // TODO probably different for mobile screens

            if (isLandscape)
            {
                var scale = Mathf.Floor((screenSize.Value.y / targetLandscapeHeight) * 2f) / 2f;
                targetDocument.panelSettings.scale = Mathf.Max(1f, scale);
            }
            else
            {
                targetDocument.panelSettings.scale = screenSize.Value.x / targetPortraitWidth;
            }

            var rootElement = targetDocument.rootVisualElement;

            rootElement.EnableInClassList("nui--landscape", isLandscape);
            rootElement.EnableInClassList("nui--portrait", !isLandscape);

            using var e = ScreenSizeChangeEvent.GetPooled();
            e.isLandscape = isLandscape;
            e.isPortrait = !isLandscape;
            e.target = rootElement;
            rootElement.SendEvent(e);
        }
    }

    public class ScreenSizeChangeEvent : EventBase<ScreenSizeChangeEvent>
    {
        public bool isPortrait;
        public bool isLandscape;

        protected override void Init()
        {
            base.Init();

            bubbles = true;
            tricklesDown = true;
            isPortrait = false;
            isLandscape = false;
        }
    }
}
