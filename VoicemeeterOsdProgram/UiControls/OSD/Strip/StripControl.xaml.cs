using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for StripControl.xaml
    /// </summary>
    public partial class StripControl : UserControl
    {
        public StripControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ShowHorizontalSeparatorAfterProperty = DependencyProperty.Register(
            "ShowHorizontalSeparatorAfter", typeof(bool), typeof(StripControl));
        public bool ShowHorizontalSeparatorAfter
        {
            get => (bool)GetValue(ShowHorizontalSeparatorAfterProperty);
            set
            {
                var thickness = BorderThickness;
                thickness.Right = value ? 1 : 0;
                BorderThickness = thickness;
                SetValue(ShowHorizontalSeparatorAfterProperty, value);
            }
        }
    }
}
