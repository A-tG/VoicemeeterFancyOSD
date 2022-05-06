using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.UiControls.OSD
{
    /// <summary>
    /// Interaction logic for OSD.xaml
    /// </summary>
    public partial class OsdControl : UserControl
    {
        public OsdControl()
        {
            InitializeComponent();
            MainContent.SizeChanged += OnSizeChange;
        }

        public bool AllowAutoUpdateSeparators { get; set; } = true;

        private double m_scale = 1;
        private double m_opacity = 1;
        private bool m_isInteractable;

        public double Scale
        {
            get => m_scale;
            set
            {
                m_scale = value;
                SetScale(value);
            }
        }

        public bool IsInteractable
        {
            get => m_isInteractable;
            set
            {
                if (m_isInteractable == value) return;

                CloseBtn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                m_isInteractable = value;
            }
        }

        public double BgOpacity
        {
            get => m_opacity;
            set
            {
                BgBorder.Background.Opacity = value;
                BgBorder.BorderBrush.Opacity = value;
            }
        }


        public void AdjustContMaxSize()
        {
            // Need to find better solution for automatic resize
            return;

            /*var scale = GetContentScaleX();
            if (scale >= 1)
            {
                MainContent.MaxWidth = MainContentWrap.MaxWidth;
                return;
            }
            // PROBLEM: When size is decreased but scale < 1 incorrect MaxWidth is assigned
            MainContent.MaxWidth = MainContentWrap.MaxWidth / scale;*/
        }

        public bool IsAnyVisibleChild()
        {
            var children = MainContent.Children;
            foreach (UIElement child in children)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateSeparators()
        {
            WrapPanelExt cont = MainContent;
            var lines = cont.GetChildrenLines();
            foreach (var line in lines)
            {
                List<UIElement> visibleElements = new();
                foreach (var el in line)
                {
                    var vis = el.Visibility;
                    if ((vis == Visibility.Visible) || (vis == Visibility.Hidden) || el.IsVisible)
                    {
                        visibleElements.Add(el);
                    }
                }
                var len = visibleElements.Count;
                if (len == 0) continue;
                foreach (StripControl item in visibleElements)
                {
                    item.ShowHorizontalSeparatorAfter = true;
                }
                var lastInLine = (StripControl)visibleElements[^1];
                lastInLine.ShowHorizontalSeparatorAfter = false;
            }
        }

        // Copied ViewboxExtensions.GetChildScaleX() from WinRTXamlToolkit project
        public double GetContentScaleX()
        {
            Viewbox viewbox = MainContentWrap;
            if (viewbox.Child == null) return 1;

            var fe = viewbox.Child as FrameworkElement;

            if ((fe is null) || (fe.ActualWidth == 0)) return 1;

            return viewbox.ActualWidth / fe.ActualWidth;
        }

        public void ReapplyScale() => SetScale(Scale);

        private void SetScale(double scale)
        {
            var elements = MainContent.Children;
            foreach (FrameworkElement element in elements)
            {
                element.LayoutTransform = new ScaleTransform(scale, scale);
            }
        }

        private void OnSizeChange(object sender, SizeChangedEventArgs e)
        {
            if (AllowAutoUpdateSeparators)
            {
                UpdateSeparators();
            }
            AdjustContMaxSize();
        }
    }
}
