using System.Drawing;
using System.Windows;
using VoicemeeterOsdProgram.UiControls;

namespace VoicemeeterOsdProgram.Factories
{
    public static class MsgBoxFactory
    {
        public static Dialog GetInfo(string title = "Info")
        {
            var d = GetCommonOk(title);
            d.SetIcon(SystemIcons.Information);
            return d;
        }

        public static Dialog GetWarning(string title = "Warning")
        {
            var d = GetCommonOk(title);
            d.SetIcon(SystemIcons.Warning);
            return d;
        }

        private static Dialog GetCommonOk(string title)
        {
            Dialog d = new() { Title = title };
            d.CancelButton.Visibility = Visibility.Collapsed;
            d.OkButton.IsCancel = true;
            return d;
        }
    }
}
