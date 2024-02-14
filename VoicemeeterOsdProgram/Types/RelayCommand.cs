using System;
using System.Windows.Input;

namespace VoicemeeterOsdProgram.Types;

public class RelayCommand : ICommand
{
    private Action<object> m_execute;
    private Func<object, bool> m_canExecute;

    public RelayCommand(Action<object> execute)
    {
        m_execute = execute;
    }

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
    {
        m_execute = execute;
        m_canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
        return (m_canExecute is null) || m_canExecute(parameter);
    }

    public void Execute(object parameter) => m_execute(parameter);
}
