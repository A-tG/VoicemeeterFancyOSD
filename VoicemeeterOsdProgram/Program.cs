using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using TopmostApp.Interop;

namespace VoicemeeterOsdProgram
{
    public class Program
    {
        public const string Name = "VoicemeeterFancyOSD";
        public const string UniqueName = "AtgDev_VoicemeeterFancyOSD";

        private static App m_app;

        [STAThread]
        static void Main(string[] args) // if *Host.exe is launched "*Host.exe" may be the args[0]
        {
#if DEBUG
            //if (!Debugger.IsAttached) Debugger.Launch();
#endif
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            ArgsHandler.HandleSpecial(args);

            if (AppLifeManager.IsAlreadyRunning && (args.Length == 0))
            {
                MessageBox.Show("The program is already running", Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Thread thread = new(async () =>
            {
                ComponentDispatcher.ThreadFilterMessage += OnTerminationSignal;
                await AppLifeManager.StartAsyn(args, () =>
                {
                    m_app = new();
                    m_app.Run();
                });
            });

            //If you launch directly from the host bridge it won't be STA.
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void ShowExceptionMsgBox(Exception ex)
        {
            try
            {
                string trace = ex?.StackTrace ?? string.Empty;
#if !DEBUG
                using var reader = new StringReader(trace);
                trace = reader.ReadLine();
#endif

                string msg = "PRESS Ctrl + C TO COPY THIS TEXT\n" +
                    "Unhandled exception:\n" +
                    $"{ex?.Message}\n" +
                    trace;
                MessageBox.Show(msg, Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        }

        private static void OnTerminationSignal(ref MSG msg, ref bool handled)
        {
            if ((msg.message == (int)WindowMessage.WM_CLOSE) || (msg.message == (int)WindowMessage.WM_QUIT))
            {
                Debug.WriteLine("TERMINATION SIGNAL RECEIVED");
                m_app?.Dispatcher.Invoke(() =>
                {
                    m_app.Shutdown();
                });
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("UNHANDLED EXCEPTION");

            var ex = e.ExceptionObject as Exception;

            ShowExceptionMsgBox(ex);
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("PROCESS EXIT");
        }
    }
}
