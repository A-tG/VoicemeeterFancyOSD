using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using VoicemeeterOsdProgram.UiControls;

namespace VoicemeeterOsdProgram.Tray
{
    public static class TrayIconManager
    {
        private static NotifyIcon m_trayIcon;
        private static ContextMenuStrip m_contextMenu;

        public static void Init()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Remove();
            App.Current.Exit += (_, _) => Remove();
            m_trayIcon = new()
            {
                Icon = Properties.Resources.MainIcon,
                Text = "Voicemeeter Fancy OSD"
            };
            ContextMenuInit();
            m_trayIcon.Visible = true;
        }

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
            context.Items.Add(new ToolStripSeparator());
#endif
            context.Items.Add(CreateExitButton());
            context.Items.Add(new ToolStripSeparator());
            context.Items.Add(CreateCloseMenuButton());
            m_contextMenu = context;
            m_trayIcon.ContextMenuStrip = context;
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
                m_debugWin.Show();
            }
            else
            {
                if (m_debugWin.IsLoaded)
                {
                    m_debugWin.Activate();
                }
                else
                {
                    m_debugWin = new DebugWindow();
                    m_debugWin.Show();
                }
            }
        }
#endif
    }
}
