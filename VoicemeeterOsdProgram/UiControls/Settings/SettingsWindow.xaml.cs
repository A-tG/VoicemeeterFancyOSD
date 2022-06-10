using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            Closing += SettingsWindow_Closing;
            InitializeComponent();

            InitOptions();
        }

        private void InitOptions()
        {
            var ch = OptionsCont.Children;

            var item = new OptionsSection();
            item.ContentContainer.Content = new Program();
            ch.Add(item);

            item = new OptionsSection();
            item.ContentContainer.Content = new Osd();
            ch.Add(item);

            item = new OptionsSection();
            item.ContentContainer.Content = new OsdAlt();
            ch.Add(item);

            item = new OptionsSection();
            item.ContentContainer.Content = new Updater();
            ch.Add(item);
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // need to hide Window instead of closing becase TabControl keeps Window in memory (internal memory leak?)
            e.Cancel = true;
            Hide();
        }
    }
}
