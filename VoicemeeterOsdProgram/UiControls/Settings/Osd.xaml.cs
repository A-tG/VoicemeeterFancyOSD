using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings;

/// <summary>
/// Interaction logic for Osd.xaml
/// </summary>
public partial class Osd : UserControl
{
    public Osd()
    {
        DataContext = OptionsStorage.Osd;

        InitializeComponent();
    }
}
