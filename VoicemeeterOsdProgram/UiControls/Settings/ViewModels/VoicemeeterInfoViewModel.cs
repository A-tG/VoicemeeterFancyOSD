using AtgDev.Voicemeeter.Types;
using System;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels
{
    public class VoicemeeterInfoViewModel : BaseViewModel, IDisposable
    {
        private string m_typeName, m_versionName = "Unknown";
        private bool m_isRunning, m_isInit;
        private bool m_isDisposed = false;

        public VoicemeeterInfoViewModel()
        {
            SetTypeName(VoicemeeterApiClient.ProgramType);
            IsRunning = VoicemeeterApiClient.IsVoicemeeterRunning;
            IsApiInit = VoicemeeterApiClient.IsInitialized;

            VoicemeeterApiClient.VoicemeeterTurnedOff += OnVoicemeeterTurned;
            VoicemeeterApiClient.VoicemeeterTurnedOn += OnVoicemeeterTurned;
            VoicemeeterApiClient.ProgramTypeChange += OnProgramTypeChanged;
            VoicemeeterApiClient.Loaded += VoicemeeterApiClient_Loaded;
        }

        private void OnVoicemeeterTurned(object sender, EventArgs e)
        {
            IsRunning = VoicemeeterApiClient.IsVoicemeeterRunning;
            SetVersionName(VoicemeeterApiClient.VoicemeeterVersion);
            SetTypeName(VoicemeeterApiClient.ProgramType);
        }

        private void VoicemeeterApiClient_Loaded(object sender, EventArgs e)
        {
            SetTypeName(VoicemeeterApiClient.ProgramType);
            SetVersionName(VoicemeeterApiClient.VoicemeeterVersion);
            IsApiInit = VoicemeeterApiClient.IsInitialized;
        }

        private void OnProgramTypeChanged(object s, VoicemeeterType type)
        {
            SetTypeName(type);
            SetVersionName(VoicemeeterApiClient.VoicemeeterVersion);
        }

        public string VersionName
        {
            get => m_versionName;
            set
            {
                m_versionName = value;
                OnPropertyChanged();
            }
        }

        public string TypeName
        {
            get => m_typeName;
            set
            {
                m_typeName = value;
                OnPropertyChanged();
            }
        }

        public bool IsApiInit
        {
            get => m_isInit;
            set
            {
                m_isInit = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get => m_isRunning;
            set
            {
                m_isRunning = value;
                OnPropertyChanged();
            }
        }

        private void SetTypeName(VoicemeeterType type)
        {
            switch (type)
            {
                case VoicemeeterType.Standard:
                case VoicemeeterType.Banana:
                    TypeName = type.ToString();
                    break;
                case VoicemeeterType.Potato64:
                case VoicemeeterType.Potato:
                    TypeName = "Potato";
                    break;
                default:
                    TypeName = "Unknown";
                    break;
            }
        }

        private void SetVersionName(VoicemeeterVersion vers)
        {
            if (vers == new VoicemeeterVersion(0))
            {
                VersionName = "Unknown";
                return;
            }
            VersionName = vers.ToString();
        }

        public void Dispose()
        {
            if (m_isDisposed) return;

            VoicemeeterApiClient.VoicemeeterTurnedOff -= OnVoicemeeterTurned;
            VoicemeeterApiClient.VoicemeeterTurnedOn -= OnVoicemeeterTurned;
            VoicemeeterApiClient.ProgramTypeChange -= OnProgramTypeChanged;
            VoicemeeterApiClient.Loaded -= VoicemeeterApiClient_Loaded;
            m_isDisposed = true;
        }
    }
}
