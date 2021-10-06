using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TopmostApp.Interop;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD
{
    public class OsdWindow : BandWindow
    {
        private const int FadeOutTimeMs = 200;

        private Rect m_workingArea;
        private VertAlignment m_vertAlign = VertAlignment.Top;
        private HorAlignment m_horAlign = HorAlignment.Left;
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

            Loaded += OnLoaded;
            SizeChanged += OnSizeChange;
            Shown += OnShow;
            SystemEvents.DisplaySettingsChanging += OnDisplaySettingsChange;
        }

        public VertAlignment WorkingAreaVertAlignment
        {
            get => m_vertAlign;
            set
            {
                m_vertAlign = value;
                UpdatePosAlign();
            }
        }

        public HorAlignment WorkingAreaHorAlignment
        {
            get => m_horAlign;
            set
            {
                m_horAlign = value;
                UpdatePosAlign();
            }
        }

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

        public Screen OnWhatDisplay()
        {
            var pointInt = new System.Drawing.Point((int)Left, (int)Top);
            return Screen.FromPoint(pointInt);
        }

        private void UpdateWorkingArea()
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            m_workingArea = Core.ScreenWorkingAreaManager.GetWokringArea();
            m_workingArea.Height /= dpi.DpiScaleY;
            m_workingArea.Width /= dpi.DpiScaleX;
            UpdateContMaxSize();
        }

        private void UpdatePosAlign()
        {
            var area = m_workingArea;
            var h = ActualHeight;
            var w = ActualWidth;
            if ((area.Height == 0) || (area.Width == 0) || (h == 0) || (w == 0)) 
                return;

            _ = WorkingAreaHorAlignment switch
            {
                HorAlignment.Left => Left = area.X,
                HorAlignment.Center => Left = area.X + (area.Width - w) / 2,
                HorAlignment.Right => Left = area.X + area.Width - w,
                _ => 0
            };
            _ = WorkingAreaVertAlignment switch
            {
                VertAlignment.Top => Top = area.Y,
                VertAlignment.Center => Top = area.Y + (area.Height - h) / 2,
                VertAlignment.Bottom => Top = area.Y + area.Height - h,
                _ => 0
            };
        }

        private void UpdateContMaxSize()
        {
            var cont = Content as OsdControl;
            if (cont is null) return;

            cont.MainContentWrap.MaxWidth = m_workingArea.Width;
            cont.MainContentWrap.MaxHeight = m_workingArea.Height;
            cont.MainContent.MaxWidth = m_workingArea.Width;
        }

        private void CancelAnimation()
        {
            m_fadeOutAnim.Completed -= OnFadeOutComplete;
            BeginAnimation(OpacityProperty, null);
            m_fadeOutAnim.Completed += OnFadeOutComplete;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            UpdateWorkingArea();
            UpdatePosAlign();
        }

        private void OnSizeChange(object sender, SizeChangedEventArgs e)
        {
            UpdatePosAlign();
        }

        private void OnDisplaySettingsChange(object sender, EventArgs e)
        {
            UpdateWorkingArea();
            UpdatePosAlign();
        }

        private void OnShow(object sender, EventArgs e)
        {
            CancelAnimation();
            UpdatePosAlign();
        }

        private void OnFadeOutComplete(object sender, EventArgs e)
        {
            Hide();
        }

        ~OsdWindow()
        {
            SystemEvents.DisplaySettingsChanging -= OnDisplaySettingsChange;
            HwndSource.Dispose();
        }
    }
}
