using AtgDev.Utils;
using System;
using System.Windows.Input;

namespace VoicemeeterOsdProgram.UiControls.Helpers
{
    public class OpenUrlCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
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

        public void Execute(object parameter)
        {
            string command = parameter as string;
            if (parameter is Uri u)
            {
                command = u.ToString();
            }
            OpenInOs.TryOpen(command);
        }
    }
}
