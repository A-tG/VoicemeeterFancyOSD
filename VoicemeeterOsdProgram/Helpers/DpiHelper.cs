using System.Windows;
using System.Windows.Media;
using VoicemeeterOsdProgram.UiControls;

namespace VoicemeeterOsdProgram.Helpers
{
    public static class DpiHelper
    {
        private static WindowExt m_dummyWindow;
        static DpiHelper()
        {
            m_dummyWindow = new()
            {
                WindowStyle = WindowStyle.None,
                IsHiddenFromTasklist = true,
                ShowActivated = false,
                ShowInTaskbar = false,
                Focusable = false,
                Width = 0,
                Height = 0
            };
        }

        public static void Init() { }

        public static DpiScale GetDpiFromPoint(Point p)
        {
            m_dummyWindow.Left = p.X;
            m_dummyWindow.Top = p.Y;
            m_dummyWindow.Show();
            var dpi = VisualTreeHelper.GetDpi(m_dummyWindow);
            m_dummyWindow.Hide();
            return dpi;
        }
    }
}
