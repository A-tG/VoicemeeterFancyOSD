using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace VoicemeeterOsdProgram.UiControls;

/// <summary>
/// Interaction logic for Dialog.xaml
/// </summary>
public partial class Dialog : Window
{
    public Dialog()
    {
        InitializeComponent();
    }

    public void SetIcon(System.Drawing.Icon icon)
    {
        var imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        DialogIcon.Source = imageSource;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) => Close();
}
