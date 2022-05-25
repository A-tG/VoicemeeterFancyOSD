using AtgDev.Utils;
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
        public enum Rate
        {
            Slow,
            Normal,
            Fast,
            VeryFast
        }

        private static Timer m_loopTimer = new()
        {
            AutoReset = true
        };
        private static VoicemeeterType m_type;
        private static bool m_isIdling;
        private static bool m_isTypeChanging;
        private static bool m_isVmRunning;
        private static bool m_isVmTurningOn;
        private static Rate m_poolingRate;
        private static bool m_isInit = false;

        private static Logger m_logger = Globals.logger;

        static VoicemeeterApiClient()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            Application.Current.DispatcherUnhandledException += (_, _) => Exit();
            Application.Current.Exit += (_, _) => Exit();

            PoolingRate = Rate.Normal;
        }

        public static async Task InitAsync(int waitTime = 0)
        {
            if (m_isInit) return;

            if (waitTime > 0)
            {
                await Task.Delay(waitTime);
            }

            await LoadAsync();

            m_isInit = true;
        }

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

        public static Rate PoolingRate
        {
            get => m_poolingRate;
            set
            {
                m_logger.Log($"VmrApi Client pooling rate is set to: {value}");

                m_poolingRate = value;
                if (IsIdling) return;

                double interval = 1000.0 / value switch
                {
                    Rate.Slow => 15,
                    Rate.Normal => 30,
                    Rate.Fast => 60,
                    _ => 30
                };
                m_loopTimer.Interval = interval;
            }
        }

        private static bool IsIdling
        {
            get => m_isIdling;
            set
            {
                if (m_isIdling == value) return;

                m_logger.Log($"VmrApi Client is idling: {value}");

                m_isIdling = value;
                if (value)
                {
                    PoolingInterval = 1000;
                }
                else
                {
                    PoolingRate = m_poolingRate;
                }
            }
        }

        private static double PoolingInterval
        {
            get => m_loopTimer.Interval;
            set => m_loopTimer.Interval = value;
        }

        public static async Task LoadAsync()
        {
            if (IsInitialized) return;

            try
            {
                m_logger?.Log("Initializing VmrApi Client");
                if (!IsLoaded)
                {
                    Api = new(PathHelper.GetDllPath());
                    var loginRes = Api.Login();

                    m_logger?.Log($"VmrApi Login result: {loginRes}");
                    if ((loginRes != ResultCodes.Ok) && (loginRes != ResultCodes.OkVmNotLaunched))
                    {
                        throw new InvalidOperationException("VmrApi is unable to login");
                    }

                    var paramsRes = await Api.WaitForNewParamsAsync(250, 1000 / 30);
                    m_logger?.Log($"VmrApi WaitForNewParams returned: {paramsRes}");

                    m_type = ProgramType;
                    m_isVmRunning = IsVoicemeeterRunning;

                    IsLoaded = true;

                    OnLoad();
                }

                m_loopTimer.Elapsed += OnTimerTick;
                m_loopTimer.Start();

                IsInitialized = true;
                m_logger?.Log("VmrApi Client initialized");
            }
            catch (Exception e)
            {
                m_logger?.LogError($"Failed to initialize VmrApi Client: {e.GetType()} {e.Message}");
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
                IsIdling = true;
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
            IsIdling = !isRunningActual;
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
            m_logger?.Log("Voicemeeter shutdown detected");
            VoicemeeterTurnedOff?.Invoke(null, EventArgs.Empty);
        }

        private static void OnVmTurnedOn()
        {
            m_logger?.Log("Voicemeeter is running");
            m_isVmTurningOn = true;
            VoicemeeterTurnedOn?.Invoke(null, EventArgs.Empty);
            m_isVmTurningOn = false;
        }

        private static void OnProgramTypeChange()
        {
            m_logger?.Log($"Voicemeeter type changed to: {ProgramType}");

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
