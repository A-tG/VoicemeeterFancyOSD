using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.UiControls;

namespace VoicemeeterOsdProgram.Tray
{
    public static class TrayIconManager
    {
        private static NotifyIcon m_trayIcon;
        private static ToolStripMenuItem m_toggleBtn;
        private static ContextMenuStrip m_contextMenu;

        static TrayIconManager()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Remove();
            App.Current.Exit += (_, _) => Remove();

            m_trayIcon = new()
            {
                Icon = Properties.Resources.MainIcon,
                Text = "Voicemeeter Fancy OSD"
            };
            ContextMenuInit();
            m_trayIcon.DoubleClick += OnToggleButton;
            m_trayIcon.Visible = true;
        }

        public static void Init() { }

        public static void Remove()
        {
            m_contextMenu?.Dispose();
            m_trayIcon?.Dispose();
        }

        private static void ContextMenuInit()
        {
            ContextMenuStrip context = new();
#if DEBUG
            context.Items.Add(CreateDebugWindow());
#endif
            context.Items.Add(CreateToggleButton());
            context.Items.Add(new ToolStripSeparator());
            context.Items.Add(CreateExitButton());
            context.Items.Add(new ToolStripSeparator());
            context.Items.Add(CreateCloseMenuButton());
            m_contextMenu = context;
            m_trayIcon.ContextMenuStrip = context;
        }

        private static ToolStripMenuItem CreateToggleButton()
        {
            ToolStripMenuItem item = new();
            item.Text = "Paused";
            item.Click += OnToggleButton;
            m_toggleBtn = item;
            return item;
        }

        private static ToolStripMenuItem CreateExitButton()
        {
            ToolStripMenuItem item = new();
            item.Text = "Exit";
            item.Click += OnExitClick;
            return item;
        }

        private static ToolStripMenuItem CreateCloseMenuButton()
        {
            ToolStripMenuItem item = new();
            item.Text = "Close Menu";
            item.Click += OnCloseClick;
            return item;
        }

        private static void OnToggleButton(object sender, EventArgs e)
        {
            if (!VoicemeeterApiClient.IsInitialized) return;

            ToggleOsdHandling();
        }

        private static void ToggleOsdHandling()
        {
            var item = m_toggleBtn;
            if (OsdWindowManager.IsEnabled)
            {
                OsdWindowManager.IsEnabled = false;
                m_trayIcon.Icon = Properties.Resources.MainIconInactive;
                item.Checked = true;
                item.Font = new(item.Font, System.Drawing.FontStyle.Bold);
            }
            else
            {
                OsdWindowManager.IsEnabled = true;
                m_trayIcon.Icon = Properties.Resources.MainIcon;
                item.Checked = false;
                item.Font = new(item.Font, System.Drawing.FontStyle.Regular);
            }
        }

        private static void OnCloseClick(object sender, EventArgs e)
        {
            m_contextMenu.Visible = false;
        }

        private static void OnExitClick(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

#if DEBUG
        private static DebugWindow m_debugWin;

        private static ToolStripMenuItem CreateDebugWindow()
        {
            ToolStripMenuItem item = new();
            item.Text = "DEBUG WINDOW";
            item.Click += OnCreateDebugWindowClick;
            return item;
        }

        private static void OnCreateDebugWindowClick(object sender, EventArgs e)
        {
            if (m_debugWin is null)
            {
                m_debugWin = new DebugWindow();
                m_debugWin.Closing += OnDebugWin_Closing;
            }
            m_debugWin.Show();
            m_debugWin.Activate();
        }

        private static void OnDebugWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            m_debugWin.Hide();
        }
#endif
    }
}
