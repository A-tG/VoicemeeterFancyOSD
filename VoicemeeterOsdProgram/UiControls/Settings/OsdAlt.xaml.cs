using AtgDev.Utils;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings;

/// <summary>
/// Interaction logic for OsdAlt.xaml
/// </summary>
public partial class OsdAlt : UserControl
{
    public OsdAlt()
    {
        DataContext = OptionsStorage.AltOsdOptionsFullscreenApps;
        InitializeComponent();
    }

    private void EditListFileButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        OpenInOs.TryOpen(Globals.FullscreenAppsListFile);
    }
}
