using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using VoicemeeterOsdProgram.Interop;
using System.Diagnostics;
using System.Windows;

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
            if (!m_mutex.WaitOne(0, false))
            {
                MessageBox.Show("Program already running", name, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

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
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("PROCESS EXIT");
        }
    }
}
