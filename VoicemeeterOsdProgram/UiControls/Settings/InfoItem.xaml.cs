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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoicemeeterOsdProgram.UiControls.Settings.ViewModels;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for InfoItem.xaml
    /// </summary>
    public partial class InfoItem : UserControl
    {
        public InfoItem()
        {
            VoicemeeterInfoViewModel vm = new();
            DataContext = vm;
            Unloaded += (_, _) => vm.Dispose();
            InitializeComponent();
        }
    }
}
