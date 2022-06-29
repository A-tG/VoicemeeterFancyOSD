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

        public static App wpf_app;

        [STAThread]
        static void Main(string[] args) // if *Host.exe is launched "*Host.exe" may be the args[0]
        {
#if DEBUG
            //if (!Debugger.IsAttached) Debugger.Launch();
#endif
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            ArgsHandler.HandleSpecial(args);

            if (AppLifeManager.IsAlreadyRunning && (args.Length == 0))
            {
                MessageBox.Show("The program is already running", Name, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Thread thread = new(() =>
            {
                ComponentDispatcher.ThreadFilterMessage += OnTerminationSignal;
                AppLifeManager.Start(args, () =>
                {
                    wpf_app = new();
                    wpf_app.Run();
                });
            });

            //If you launch directly from the host bridge it won't be STA.
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void OnTerminationSignal(ref MSG msg, ref bool handled)
        {
            if ((msg.message == (int)WindowMessage.WM_CLOSE) || (msg.message == (int)WindowMessage.WM_QUIT))
            {
                Debug.WriteLine("TERMINATION SIGNAL RECEIVED");
                wpf_app?.Dispatcher.Invoke(() =>
                {
                    wpf_app.Shutdown();
                });
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("UNHANDLED EXCEPTION");
            try
            {
                var ex = e.ExceptionObject as Exception;
                var path = AppDomain.CurrentDomain.BaseDirectory + @"\crashes";
                var filePath = path + @$"\crash {DateTime.Now:dd-MM-yyyy HH-mm-ss}.log";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using StreamWriter sr = new(filePath);
                sr.WriteLine($"{ex.GetType}\n{ex.Message}\n{ex.StackTrace}");
            }
            catch { }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("PROCESS EXIT");
        }
    }
}
