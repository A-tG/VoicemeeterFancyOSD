using AtgDev.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private PeriodicTimerExt m_movedTimer = new(TimeSpan.FromSeconds(2));
        private SettingsWindowViewModel m_model = new();

        public SettingsWindow()
        {
            DataContext = m_model;
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private async void OnInitialized(object sender, EventArgs e)
        {
            await m_model.TryReadWindowSettings();
            MoveToWorkingArea();
        }

        private void MoveToWorkingArea()
        {
            var right = Left + ActualWidth;
            var bottom = Top - ActualHeight;
            var allScreens = Screen.AllScreens;
            bool isLTonScreen = !allScreens.All(s => !s.WorkingArea.Contains(new Point(Left, Top)));
            bool isRTonScreen = !allScreens.All(s => !s.WorkingArea.Contains(new Point(right, Top)));
            bool isTitleBarOnScreen = isLTonScreen && isRTonScreen;
            if (isTitleBarOnScreen) return;

            List<Screen> screens = new(new[]
            {
                Screen.FromPoint(new Point(Left, Top)),
                Screen.FromPoint(new Point(right, Top)),
                Screen.FromPoint(new Point(right, bottom)),
                Screen.FromPoint(new Point(Left, bottom))
            });
            var screen = screens.GroupBy(s => s).OrderByDescending(g => g.Count()).First().Key;
            Left = screen.WorkingArea.Left;
            Top = screen.WorkingArea.Top;
        }

        private void Center()
        {
            var screen = Screen.FromPoint(new Point(Left, Top));
            var h = screen.WorkingArea.Height;
            var w = screen.WorkingArea.Width;
            var top = screen.WorkingArea.Top;
            var left = screen.WorkingArea.Left;
            UpdateLayout();
            if ((ActualHeight > h) || (ActualWidth > w))
            {
                WindowState = WindowState.Maximized;
                return;
            }
            Left = (w - ActualWidth) / 2;
            Top = (h - ActualHeight) / 2;
        }

        private async void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded) return;

            m_movedTimer.Start();
            if (await m_movedTimer.WaitForNextTickAsync())
            {
                await m_model.TrySaveWindowSettings();
            }

        }

        private async void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            m_movedTimer.Stop();
            await m_model.TrySaveWindowSettings();
            // need to hide Window instead of closing becase TabControl keeps Window in memory (internal memory leak?)
            Hide();
        }

        private void OpenConfigFileClick(object sender, RoutedEventArgs e)
        {
            OpenInOs.TryOpen(OptionsStorage.ConfigFilePath);
        }

        private void OpenConfigFolderClick(object sender, RoutedEventArgs e)
        {
            OpenInOs.TryOpen(OptionsStorage.ConfigFolder);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            m_model.Close();
        }

        private async void OnLocationChange(object sender, EventArgs e)
        {
            if (!IsLoaded) return;

            m_movedTimer.Start();
            if (await m_movedTimer.WaitForNextTickAsync())
            {
                await m_model.TrySaveWindowSettings();
            }
        }
    }
}
