using AtgDev.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.UiControls.Helpers;
using WpfScreenHelper;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private PeriodicTimerExt m_movedTimer = new(TimeSpan.FromSeconds(2));
        private WindowPersistence m_pers;

        public SettingsWindow()
        {
            m_pers = new(this, Path.Combine(OptionsStorage.ConfigFolder, "SettingsWindow"))
            {
                logger = Globals.logger
            };
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private async void OnInitialized(object sender, EventArgs e)
        {
            await m_pers.TryReadWindowSettingsAsync();
            MoveToWorkingArea();
        }

        private void MoveToWorkingArea()
        {
            var right = Left + ActualWidth;
            var bottom = Top - ActualHeight;
            var allScreens = Screen.AllScreens;
            bool isLtOnScreen = !allScreens.All(s => !s.WorkingArea.Contains(new Point(Left, Top)));
            bool isRtOnScreen = !allScreens.All(s => !s.WorkingArea.Contains(new Point(right, Top)));
            bool isTitleBarOnScreen = isLtOnScreen && isRtOnScreen;
            if (isTitleBarOnScreen) return;

            List<Screen> screens = new(new[]
            {
                Screen.FromPoint(new Point(Left, Top)),
                Screen.FromPoint(new Point(right, Top)),
                Screen.FromPoint(new Point(right, bottom)),
                Screen.FromPoint(new Point(Left, bottom))
            });
            var screenWithMostCorners = screens.GroupBy(s => s).OrderByDescending(g => g.Count()).First().Key;
            Left = screenWithMostCorners.WorkingArea.Left;
            Top = screenWithMostCorners.WorkingArea.Top;
        }

        private void Center()
        {
            var screen = Screen.FromPoint(new Point(Left, Top));
            var h = screen.WorkingArea.Height;
            var w = screen.WorkingArea.Width;
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
                _ = await m_pers.TrySaveWindowSettingsAsync();
            }

        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            m_movedTimer.Stop();
            // need to hide Window instead of closing becase TabControl keeps Window in memory (internal memory leak?)
            Hide();

            OptionsStorage.TrySave();
            m_pers.TrySaveWindowSettings();
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
            m_pers.Dispose();
        }

        private async void OnLocationChange(object sender, EventArgs e)
        {
            if (!IsLoaded) return;

            m_movedTimer.Start();
            if (await m_movedTimer.WaitForNextTickAsync())
            {
                await m_pers.TrySaveWindowSettingsAsync();
            }
        }
    }
}
