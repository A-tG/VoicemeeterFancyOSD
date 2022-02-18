using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using TopmostApp.Helpers;
using TopmostApp.Interop;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Helpers;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Types;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.UiControls.OSD
{
    public class OsdWindow : BandWindow
    {
        private const int FadeOutTimeMs = 200;

        private Rect m_workingArea;
        private DoubleAnimation m_fadeOutAnim;

        public OsdWindow() : base()
        {
            var anim = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn },
                Duration = new Duration(TimeSpan.FromMilliseconds(FadeOutTimeMs)),
                FillBehavior = FillBehavior.Stop
            };
            anim.Completed += OnFadeOutComplete;
            m_fadeOutAnim = anim;

            Loaded += (_, _) => UpdatePos();
            SizeChanged += (_, _) => UpdatePosAlign();
            Globals.Osd.screenProvider.MainScreenChanged += OnEventUpdatePos;
            Globals.Osd.fullscreenAppsWatcher.IsDetectedChanged += OnEventUpdatePos;
            InitAlignEvents();

            // triggered if any setting is changed including taskbar resize, display resolution
            SystemEvents.UserPreferenceChanged += OnEventUpdatePos;

            Unloaded += OsdWindow_Unloaded;
        }

        private void InitAlignEvents()
        {
            var osdMainOpts = OptionsStorage.Osd;
            var osdAltOpts = OptionsStorage.AltOsdOptionsFullscreenApps;
            osdMainOpts.HorizontalAlignmentChanged += OnEventUpdatePosAlign;
            osdMainOpts.VerticalAlignmentChanged += OnEventUpdatePosAlign;
            osdAltOpts.HorizontalAlignmentChanged += OnEventUpdatePosAlign;
            osdAltOpts.VerticalAlignmentChanged += OnEventUpdatePosAlign;
        }

        public VertAlignment WorkingAreaVertAlignment => Globals.Osd.fullscreenAppsWatcher.IsDetected ?
            OptionsStorage.AltOsdOptionsFullscreenApps.VerticalAlignment :
            OptionsStorage.Osd.VerticalAlignment;

        public HorAlignment WorkingAreaHorAlignment => Globals.Osd.fullscreenAppsWatcher.IsDetected ?
            OptionsStorage.AltOsdOptionsFullscreenApps.HorizontalAlignment :
            OptionsStorage.Osd.HorizontalAlignment;

        public void HideAnimated(uint duration = FadeOutTimeMs)
        {
            if (duration > 0)
            {
                m_fadeOutAnim.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                BeginAnimation(OpacityProperty, m_fadeOutAnim);
            }
            else
            {
                Hide();
            }
        }

        public Screen OnWhatDisplay() => Screen.FromPoint(new Point(Left, Top));

        public new void Show()
        {
            CancelAnimation();
            base.Show();
        }

        private void UpdateWorkingArea()
        {
            m_workingArea = Globals.Osd.workingAreaProvider.GetWokringArea();

            var dpi = DpiHelper.GetDpiFromPoint(new Point(m_workingArea.X, m_workingArea.Y));
            m_workingArea.Width /= dpi.DpiScaleX;
            m_workingArea.Height /= dpi.DpiScaleY;
            UpdateContMaxSize();
        }

        private void UpdatePosAlign()
        {
            var area = m_workingArea;
            var h = ActualHeight;
            var w = ActualWidth;
            if ((area.Height == 0) || (area.Width == 0) || (h == 0) || (w == 0)) 
                return;

            var dpi = DpiHelper.GetDpiFromPoint(new Point(area.X, area.Y));
            var scaleX = 1 / dpi.DpiScaleX;
            var scaleY = 1 / dpi.DpiScaleY;

            _ = WorkingAreaHorAlignment switch
            {
                HorAlignment.Left => Left = area.X,
                HorAlignment.Center => Left = area.X + (area.Width - w) / 2 / scaleX,
                HorAlignment.Right => Left = area.X + (area.Width - w) / scaleX,
                _ => 0
            };
            _ = WorkingAreaVertAlignment switch
            {
                VertAlignment.Top => Top = area.Y,
                VertAlignment.Center => Top = area.Y + (area.Height - h) / 2 / scaleY,
                VertAlignment.Bottom => Top = area.Y + (area.Height - h) / scaleY,
                _ => 0
            };
        }

        private void UpdateContMaxSize()
        {
            if (Content is not OsdControl cont) return;

            cont.MainContentWrap.MaxWidth = m_workingArea.Width;
            cont.MainContentWrap.MaxHeight = m_workingArea.Height;
            cont.MainContent.MaxWidth = m_workingArea.Width;
        }

        private void UpdatePos()
        {
            UpdateWorkingArea();
            UpdatePosAlign();
        }

        private void CancelAnimation()
        {
            m_fadeOutAnim.Completed -= OnFadeOutComplete;
            BeginAnimation(OpacityProperty, null);
            m_fadeOutAnim.Completed += OnFadeOutComplete;
        }

        // do unsubcribing really works?
        private void OnEventUpdatePos<T>(object sender, T e) => UpdatePos();
        private void OnEventUpdatePosAlign<T>(object sender, T e) => UpdatePosAlign();

        private void OnSystemSettingsChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            UpdatePos();
        }

        private void OnFadeOutComplete(object sender, EventArgs e) => Hide();

        private void OsdWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.Osd.fullscreenAppsWatcher.IsDetectedChanged -= OnEventUpdatePos;
            Globals.Osd.screenProvider.MainScreenChanged -= OnEventUpdatePos;
            OptionsStorage.Osd.HorizontalAlignmentChanged -= OnEventUpdatePosAlign;
            OptionsStorage.Osd.VerticalAlignmentChanged -= OnEventUpdatePosAlign;
            OptionsStorage.AltOsdOptionsFullscreenApps.HorizontalAlignmentChanged -= OnEventUpdatePosAlign;
            OptionsStorage.AltOsdOptionsFullscreenApps.VerticalAlignmentChanged -= OnEventUpdatePosAlign;
            SystemEvents.UserPreferenceChanged -= OnEventUpdatePos;
        }
    }
}
