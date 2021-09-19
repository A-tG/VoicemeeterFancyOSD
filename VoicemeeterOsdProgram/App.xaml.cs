using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VoicemeeterOsdProgram.Tray;

namespace VoicemeeterOsdProgram
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        void OnAppStartup(object sender, StartupEventArgs e)
        {
            TrayIconManager.Init();
            Core.OsdWindowManager.Init();
        }
    }
}
