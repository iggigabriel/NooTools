using Noo.Nui;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiConsoleLogDrawer : NuiDrawer
    {
        public class LogMsg
        {
            public DateTime time;
            public string preview;
            public string message;
            public string stackTrace;
            public LogType type;
        }

        readonly CircularBuffer<LogMsg> logs = new(1000);
        readonly VisualElement dummy;
        readonly NuiListView<LogMsg> list;
        readonly VisualElement overlay;
        readonly TextElement overlayText;
        readonly NuiButton overlayCloseBtn;
        bool isCreated;

        public NuiConsoleLogDrawer()
        {
            Application.logMessageReceived += LogMessageReceived;

            dummy = new VisualElement();
            dummy.style.width = 309;
            dummy.style.height = 372;

            list = new NuiListView<LogMsg>(OnLogCraete, OnLogDestroy, OnLogFilter, 20);
            list.AddToClassList("dtk-console-log-list");

            overlay = new VisualElement();
            overlay.AddToClassList("dtk-console-log-overlay");

            var scrollView = new ScrollView().AppendTo(overlay);
            scrollView.AddToClassList("dtk-console-log-overlay-frame");

            overlayText = new TextElement().AppendTo(scrollView);
            overlayText.AddToClassList("dtk-console-log-overlay-text");

            overlayCloseBtn = new NuiButton().WithClass("nui-btn-black", "flex-grow").AppendTo(overlay);
            overlayCloseBtn.ButtonText = "Close";
            overlayCloseBtn.AddToClassList("dtk-console-log-overlay-btn-close");

            overlay.style.display = DisplayStyle.None;
        }

        private bool OnLogFilter(LogMsg item, string query)
        {
            return true;
        }

        private VisualElement OnLogCraete(LogMsg data, int index)
        {
            var txt = NuiPool.Rent<NuiText>();
            txt.AddToClassList("dtk-console-log-text");
            if (index % 2 == 0) txt.AddToClassList("dtk-odd");
            txt.text = data.preview;
            txt.userData = data;

            switch (data.type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    txt.AddToClassList("text-light-red");
                    break;
                case LogType.Warning:
                    txt.AddToClassList("text-yellow");
                    break;
            }

            return txt;
        }

        private void OnLogDestroy(LogMsg data, VisualElement element)
        {
            element.ClearClassList();
            NuiPool.Return(element);
        }

        private void LogMessageReceived(string message, string stackTrace, LogType type)
        {
            logs.PushFront(new LogMsg
            {
                time = DateTime.Now,
                preview = FirstLine(message).ToString(),
                message = message,
                stackTrace = stackTrace,
                type = type
            });

            if (isCreated)
            {
                list.SetItems(logs);
            }
        }

        protected override void OnCreate()
        {
            dummy.AppendTo(Root);
            list.AppendTo(Root);
            overlay.AppendTo(Root);

            list.SetItems(logs);

            overlayCloseBtn.clicked += () =>
            {
                overlay.style.display = DisplayStyle.None;
            };

            AddDragScroll(list.scrollView);
            isCreated = true;
        }

        protected override void OnDestroy()
        {
            dummy.RemoveFromHierarchy();
            list.RemoveFromHierarchy();
            overlay.RemoveFromHierarchy();
            isCreated = false;
        }

        private void InitializeOverlay(LogMsg data)
        {
            overlay.style.display = DisplayStyle.Flex;
            overlayText.text = $"[{data.time:HH:mm:ss}] {data.message}\n\nStacktrace:\n{data.stackTrace}";
        }

        static ReadOnlySpan<char> FirstLine(ReadOnlySpan<char> text)
        {
            int first = text.IndexOf('\n');
            if (first == -1) return text;
            return text[..(first)];
        }

        static ReadOnlySpan<char> FirstTwoLines(ReadOnlySpan<char> text)
        {
            int first = text.IndexOf('\n');
            if (first == -1) return text;

            int second = text[(first + 1)..].IndexOf('\n');
            if (second == -1) return text;

            return text[..(first + 1 + second)];
        }

        void AddDragScroll(ScrollView scrollView)
        {
            Vector2 startPointerPos = default;
            Vector2 startScrollOffset = default;
            bool isDragging = default;

            scrollView.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.target is VisualElement el)
                {
                    foreach (var child in el.Children())
                    {
                        if (child.userData is LogMsg logMsg)
                        {
                            list.userData = logMsg;
                        }
                    }
                }

                isDragging = true;
                startPointerPos = evt.position;
                startScrollOffset = scrollView.scrollOffset;
                scrollView.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            });

            scrollView.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!isDragging) return;

                Vector2 delta = (Vector2)evt.position - startPointerPos;
                scrollView.scrollOffset = startScrollOffset - delta;
                evt.StopPropagation();
            });

            scrollView.RegisterCallback<PointerUpEvent>(evt =>
            {
                Vector2 delta = (Vector2)evt.position - startPointerPos;

                if (Mathf.Abs(delta.y) < 15)
                {
                    if (evt.target is VisualElement el)
                    {
                        foreach (var child in el.Children())
                        {
                            if (child.userData is LogMsg logMsg && list.userData == logMsg)
                            {
                                InitializeOverlay(logMsg);
                            }
                        }
                    }
                }

                list.userData = null;

                isDragging = false;
                scrollView.ReleasePointer(evt.pointerId);
                evt.StopPropagation();
            });

            scrollView.RegisterCallback<PointerCancelEvent>(evt =>
            {
                isDragging = false;
                scrollView.ReleasePointer(evt.pointerId);
            });
        }

    }
}
