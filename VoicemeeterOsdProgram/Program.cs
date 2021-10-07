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
        public const string name = "VoicemeeterFancyOSD";

        private static App m_app;
        private static Mutex m_mutex = new(true, "Atg_VoicemeeterFancyOSD");

        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            //if (!Debugger.IsAttached) Debugger.Launch();
#endif
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            if (!m_mutex.WaitOne(0, false))
            {
                MessageBox.Show("The program is already running", name, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Thread thread = new(() =>
            {
                ComponentDispatcher.ThreadFilterMessage += OnTerminationSignal;
                m_app = new App();
                m_app.Run();
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
                m_app.Dispatcher.Invoke(() =>
                {
                    m_app.Shutdown();
                });
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("UNHANDLED EXCEPTION");

            var ex = e.ExceptionObject as Exception;

            string trace = ex?.StackTrace ?? string.Empty;
#if !DEBUG
            using var reader = new StringReader(trace);
            trace = reader.ReadLine();
#endif

            string msg = "PRESS Ctrl + C TO COPY THIS TEXT\n" +
                "Unhandled exception:\n" + 
                $"{ex?.Message}\n" + 
                trace;
            MessageBox.Show(msg, name, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("PROCESS EXIT");
        }
    }
}
