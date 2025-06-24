using Noo.Nui;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public partial class DtkCommandsPage : DtkPage
    {
        public override string Title => "Commands";
        public override MatIcon Icon => MatIcon.Wysiwyg;

        readonly NuiToolbar toolbar;

        readonly Button toolbarBackBtn;
        readonly Button toolbarMoreBtn;
        readonly NuiSearchField searchField;

        readonly Stack<DevToolkitCommands.CommandPage> activePages = new();
        DevToolkitCommands.CommandPage activePage;

        DtkInspectorView previousInspectorView;
        DtkInspectorView activeInspectorView;

        readonly DtkInspectorView inspectorViewA;
        readonly DtkInspectorView inspectorViewB;
        readonly DtkInspectorView inspectorSearch;

        public event Action<IReadOnlyList<string>> OnSearchQuery;
        public event Action<Button> OnMoreClicked;

        bool searchActive;

        public DtkCommandsPage() : base()
        {
            toolbar = new NuiToolbar().AppendTo(this);

            toolbar.left.Add(toolbarBackBtn = new NuiToolbarButton(MatIcon.ArrowBackIosNew));
            toolbar.right.Add(toolbarMoreBtn = new NuiToolbarButton(MatIcon.MoreVert));

            searchField = new NuiSearchField().WithClass("ml-4", "mr-4").AppendTo(toolbar.middle);

            var inspectorBase = new VisualElement().WithClass("flex-grow", "overflow-hidden").AppendTo(this);

            inspectorViewA = new DtkInspectorView().WithName("inspectorViewA").AppendTo(inspectorBase);
            inspectorViewB = new DtkInspectorView().WithName("inspectorViewB").AppendTo(inspectorBase);
            inspectorSearch = new DtkInspectorView().WithName("inspectorSearch").AppendTo(inspectorBase);
        }

        private void ShowMore()
        {
            OnMoreClicked?.Invoke(toolbarMoreBtn);
        }

        private void OnSearchQueryChanged(ChangeEvent<string> e)
        {
            var queries = DevToolkitUtility.ParseSearchQuery(e.newValue);
            OnSearchQuery?.Invoke(queries);
        }

        public void ShowFoundCommands(IReadOnlyList<NuiDrawer> drawers)
        {
            if (drawers == null || drawers.Count == 0)
            {
                if (!searchActive) return;

                searchActive = false;
                inspectorSearch.Hide();

                if (activeInspectorView != null && activePage != null)
                {
                    activeInspectorView.SetDrawers(activePage.drawers);
                    activeInspectorView.Show();
                }
            }
            else
            {
                searchActive = true;

                activeInspectorView?.Hide();

                inspectorSearch.Show();
                inspectorSearch.SetDrawers(drawers);
            }

            UpdateBackButton();
        }

        internal void GoBack()
        {
            if (searchActive)
            {
                searchField.value = string.Empty;
                ShowFoundCommands(null);
                return;
            }

            if (activePages.Count > 1)
            {
                activePages.Pop();
                SetActivePage(activePages.Peek(), false);
                UpdateBackButton();
            }
        }

        internal void ShowCommandPage(DevToolkitCommands.CommandPage page)
        {
            if (searchActive) searchField.value = string.Empty;
            if (page == null) return;
            if (activePages.Count > 0 && activePages.Peek() == page) return;

            if (activePages.Contains(page))
            {
                while (activePages.Peek() != page) activePages.Pop();
                SetActivePage(page, false);
            }
            else
            {
                activePages.Push(page);
                SetActivePage(page, true);
            }

            UpdateBackButton();
        }

        private void UpdateBackButton()
        {
            toolbarBackBtn.enabledSelf = activePages.Count > 1 || searchActive;
        }

        private void SetActiveView(DtkInspectorView view, bool showFromRight)
        {
            if (activeInspectorView == view) return;

            previousInspectorView = activeInspectorView;
            activeInspectorView = view;

            previousInspectorView?.Hide();
            activeInspectorView.Show();
        }

        private void SetActivePage(DevToolkitCommands.CommandPage page, bool showFromRight)
        {
            if (activeInspectorView != inspectorViewA)
            {
                SetActiveView(inspectorViewA, showFromRight);
            }
            else
            {
                SetActiveView(inspectorViewB, showFromRight);
            }

            page.AssertSorted();

            activePage = page;

            activeInspectorView.SetDrawers(page.drawers);
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            NuiTask.OnInterval(NuiTask.ExecutionOrder.Update, 1f / 12f, false, UpdateDrawers);

            toolbarBackBtn.clicked += GoBack;
            toolbarMoreBtn.clicked += ShowMore;
            searchField.RegisterValueChangedCallback(OnSearchQueryChanged);

            inspectorViewA.Hide();
            inspectorViewB.Hide();
            inspectorSearch.Hide();
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            NuiTask.Cancel(UpdateDrawers);
            searchField.value = string.Empty;
            searchActive = false;
            activeInspectorView = null;
            previousInspectorView = null;

            toolbarBackBtn.clicked -= GoBack;
            toolbarMoreBtn.clicked -= ShowMore;
            searchField.UnregisterValueChangedCallback(OnSearchQueryChanged);

            activePage = null;
            activePages.Clear();

            inspectorViewA.Hide();
            inspectorViewB.Hide();
            inspectorSearch.Hide();
        }

        void UpdateDrawers()
        {
            if (activePage != null)
            {
                foreach (var drawer in activePage.drawers)
                {
                    drawer.Update();
                }
            }
        }
    }
}
