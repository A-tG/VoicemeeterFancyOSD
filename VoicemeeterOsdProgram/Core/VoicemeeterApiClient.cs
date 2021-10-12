using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Types;
using AtgDev.Voicemeeter.Utils;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace VoicemeeterOsdProgram.Core
{
    public static class VoicemeeterApiClient
    {
        private const double SlowTickTimeMs = 1000;
        private const double NormalTickTime = 1000 / 30;
        private const double FastTickTimeMs = 1000 / 60;

        private static Timer m_loopTimer = new()
        {
            AutoReset = true,
            Interval = NormalTickTime
        };
        private static VoicemeeterType m_type;
        private static bool m_isSlowUpdate;
        private static bool m_isTypeChanging;
        private static bool m_isVmRunning;
        private static bool m_isVmTurningOn;

        static VoicemeeterApiClient()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            Application.Current.Exit += (_, _) => Exit();

            _ = LoadAsync();
        }

        public static void Init() { }

        public static RemoteApiExtender Api { get; private set; }

        public static bool IsLoaded { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static bool IsVoicemeeterRunning 
        {
            get => Api?.GetVoicemeeterType(out _) == 0;
            private set
            {
                if (m_isVmRunning == value) return;

                m_isVmRunning = value;
                if (value)
                {
                    OnVmTurnedOn();
                }
                else
                {
                    OnVmTurnedOff();
                }
            }
        }

        public static bool IsHandlingParams { get; set; } = true;

        public static VoicemeeterType ProgramType
        {
            get
            {
                var type = VoicemeeterType.None;
                bool isCompleted = Api?.GetVoicemeeterType(out type) == 0;
                return isCompleted ? type : VoicemeeterType.None;
            }
            private set
            {
                if ((value == VoicemeeterType.None) || (m_type == value)) return;

                if (m_type != VoicemeeterType.None)
                {
                    OnProgramTypeChange();
                }
                m_type = value;
            }
        }

        private static double TickTime
        {
            get => m_loopTimer.Interval;
            set
            {
                if ((m_loopTimer.Interval != value) && (value > 0))
                {
                    m_loopTimer.Interval = value;
                }
            }
        }

        private static bool IsSlowUpdate
        {
            get => m_isSlowUpdate;
            set
            {
                m_isSlowUpdate = value;
                TickTime = value ? SlowTickTimeMs : NormalTickTime;
            }
        }

        public static async Task LoadAsync()
        {
            if (IsInitialized) return;

            try
            {
                if (!IsLoaded)
                {
                    Api = new(PathHelper.GetDllPath());
                    Api.Login();
                    _ = await Api.WaitForNewParamsAsync(250, 1000 / 30);
                    m_type = ProgramType;
                    m_isVmRunning = IsVoicemeeterRunning;

                    IsLoaded = true;

                    OnLoad();
                }

                m_loopTimer.Elapsed += OnTimerTick;
                m_loopTimer.Start();

                IsInitialized = true;

            }
            catch
            {
                if (!IsLoaded)
                {
                    Api?.Dispose();
                    Api = null;
                }

                if (m_loopTimer is not null)
                {
                    m_loopTimer.Stop();
                    m_loopTimer.Elapsed -= OnTimerTick;
                }
            }
        }

        public static void Exit()
        {
            m_loopTimer?.Stop();
            Api?.Logout();
        }

        private static void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            HandleServerConnection();

            if (!IsHandlingParams)
            {
                _ = Api.IsParametersDirty();
                IsSlowUpdate = true;
                return;
            }

            if (!m_isVmRunning && m_isVmTurningOn && m_isTypeChanging) return;

            HandleProgramType();
            HandleParameters();
        }

        private static void HandleServerConnection()
        {
            if (m_isVmTurningOn) return;

            bool isRunningActual = IsVoicemeeterRunning;
            IsSlowUpdate = !isRunningActual;
            IsVoicemeeterRunning = isRunningActual;
        }

        private static void HandleProgramType()
        {
            if (m_isTypeChanging) return;

            var actualType = ProgramType;
            ProgramType = actualType;
        }

        private static void HandleParameters()
        {
            if (Api.IsParametersDirty() == 1)
            {
                OnNewParameters();
            }
        }

        private static event EventHandler m_loaded;

        public static event EventHandler NewParameters;
        public static event EventHandler<VoicemeeterType> ProgramTypeChange;
        public static event EventHandler VoicemeeterTurnedOff;
        public static event EventHandler VoicemeeterTurnedOn;

        public static event EventHandler Loaded
        {
            add
            {
                if (IsLoaded)
                {
                    value?.Invoke(null, EventArgs.Empty);
                }
                else
                {
                    m_loaded += value;
                }
            }
            remove => m_loaded -= value;
        }

        private static void OnNewParameters()
        {
            NewParameters?.Invoke(null, EventArgs.Empty);
        }

        private static void OnLoad()
        {
            m_loaded?.Invoke(null, EventArgs.Empty);
        }

        private static void OnVmTurnedOff()
        {
            VoicemeeterTurnedOff?.Invoke(null, EventArgs.Empty);
        }

        private static void OnVmTurnedOn()
        {
            m_isVmTurningOn = true;
            VoicemeeterTurnedOn?.Invoke(null, EventArgs.Empty);
            m_isVmTurningOn = false;
        }

        private static void OnProgramTypeChange()
        {
            m_isTypeChanging = true;
           
            var type = ProgramType;
            if (ProgramType != VoicemeeterType.None)
            {
                ProgramTypeChange?.Invoke(null, ProgramType);
            }

            m_isTypeChanging = false;
        }
    }
}
