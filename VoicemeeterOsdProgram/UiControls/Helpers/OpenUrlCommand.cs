using AtgDev.Utils;
using System;
using System.Windows.Input;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Helpers;

public class OpenUrlCommand : RelayCommand
{
    public OpenUrlCommand() : base(OpenUri, CanOpenUri) { }

    private static bool CanOpenUri(object parameter)
    {
        if (parameter is Uri u)
        {
            return !string.IsNullOrEmpty(u.ToString());
        }
        else if (parameter is string s)
        {
            return !string.IsNullOrEmpty(s);
        }
        return false;
    }

    private static void OpenUri(object parameter)
    {
        string command = parameter as string;
        if (parameter is Uri u)
        {
            command = u.ToString();
        }
        OpenInOs.TryOpen(command);
    }
}
