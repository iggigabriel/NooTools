using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public class NuiScrollView : ScrollView, INuiPoolable
    {
        public const float MouseWheenScrollSize = 8;

        public static void InitializeScrollView(ScrollView scrollView)
        {
            scrollView.AddToClassList("nui-scrollview");
            scrollView.mouseWheelScrollSize = MouseWheenScrollSize;
            scrollView.verticalScroller.highButton.Add(new NuiIconMat(MatIcon.ArrowDropDown).WithClass("nui-scroller-btn-icon"));
            scrollView.verticalScroller.lowButton.Add(new NuiIconMat(MatIcon.ArrowDropUp).WithClass("nui-scroller-btn-icon"));
            scrollView.touchScrollBehavior = TouchScrollBehavior.Clamped;
        }

        public NuiScrollView() : this(ScrollViewMode.Vertical)
        {
        }

        public NuiScrollView(ScrollViewMode scrollViewMode) : base(scrollViewMode)
        {
            InitializeScrollView(this);
        }

        public void OnRentFromPool()
        {
        }

        public void OnReturnToPool()
        {
            scrollOffset = default;
            Assert.AreEqual(childCount, 0, "NuiScrollView should not have any children when returning to pool.");
        }
    }
}
