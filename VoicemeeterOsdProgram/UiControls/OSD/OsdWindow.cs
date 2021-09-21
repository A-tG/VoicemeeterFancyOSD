using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VoicemeeterOsdProgram.Interop;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD
{
    public class OsdWindow : BandWindow
    {
        public OsdWindow() : base()
        {
            var anim = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut },
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

        private const int FadeOutTimeMs = 200;

        private Rect m_workingArea;
        private VertAlignment m_vertAlign = VertAlignment.Top;
        private HorAlignment m_horAlign = HorAlignment.Left;
        private DoubleAnimation m_fadeOutAnim;

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

        public void HideAnimated()
        {
            BeginAnimation(OpacityProperty, m_fadeOutAnim);
        }

        public Screen OnWhatDisplay()
        {
            var pointInt = new System.Drawing.Point((int)Left, (int)Top);
            return Screen.FromPoint(pointInt);
        }

        private void UpdateWorkingArea()
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var area = Core.ScreenWorkingAreaManager.GetWokringArea();
            m_workingArea.Height = area.Height / dpi.DpiScaleX;
            m_workingArea.Width = area.Width / dpi.DpiScaleY;
            m_workingArea.X = area.X;
            m_workingArea.Y = area.Y;
            UpdateContMaxSize();
        }

        private void UpdatePosAlign()
        {
            var area = m_workingArea;
            var h = ActualHeight;
            var w = ActualWidth;
            if ((area.Height == 0) || (area.Width == 0) || (h == 0) || (w == 0)) 
                return;

            switch (WorkingAreaHorAlignment)
            {
                case HorAlignment.Left:
                    Left = area.X;
                    break;
                case HorAlignment.Center:
                    Left = area.X + (area.Width - w) / 2;
                    break;
                case HorAlignment.Right:
                    Left = area.X + area.Width - w;
                    break;
            }
            switch (WorkingAreaVertAlignment)
            {
                case VertAlignment.Top:
                    Top = area.Y;
                    break;
                case VertAlignment.Center:
                    Top = area.Y + (area.Height - h) / 2;
                    break;
                case VertAlignment.Bottom:
                    Top = area.Y + area.Height - h;
                    break;
            }
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
