using AtgDev.Voicemeeter.Types;
using System;
using System.Windows;

namespace VoicemeeterOsdProgram.Core
{
    partial class OsdWindowManager
    {
        private static void OnVoicemeeterTypeChange(object sender, VoicemeeterType t)
        {
            if (t == VoicemeeterType.None) return;

            m_isVmTypeChanging = true;
            m_WaitForVmTypeTimer.Stop();
            m_WaitForVmTypeTimer.Start();
        }

        private static void OnVoicemeeterLoad(object sender, EventArgs e)
        {
            var dis = Application.Current.Dispatcher;
            VoicemeeterApiClient.ProgramTypeChange += OnVoicemeeterTypeChange;
            VoicemeeterApiClient.NewParameters += (_, _) => dis.Invoke(UpdateOsd);
            VoicemeeterApiClient.VoicemeeterTurnedOff += (_, _) => dis.Invoke(OnVoicemeeterTurnedOff);
            VoicemeeterApiClient.VoicemeeterTurnedOn += (_, _) => dis.Invoke(OnVoicemeeterTurnedOn);

            var type = VoicemeeterApiClient.ProgramType;
            if (type == VoicemeeterType.None)
            {
                VoicemeeterApiClient.VoicemeeterTurnedOn += OnLateVmStart;
                return;
            }

            RefillOsd(type);
        }

        private static void WaitForVmTypeTimerTick(object sender, EventArgs e)
        {
            m_WaitForVmTypeTimer.Stop();
            RefillOsd(VoicemeeterApiClient.ProgramType);
            m_isVmTypeChanging = false;
        }

        private static void WaitForVmTimerTick(object sender, EventArgs e)
        {
            m_WaitForVmStartedTimer.Stop();
            m_isVmStarting = false;
        }

        private static void OnVoicemeeterTurnedOn()
        {
            // workaround to prevent showing bugged parameters after Voicemeeter launch
            m_isVmStarting = true;
            m_WaitForVmStartedTimer.Stop();
            m_WaitForVmStartedTimer.Start();
        }

        private static void OnVoicemeeterTurnedOff()
        {
            // workaround to prevent showing bugged parameters after Voicemeeter shut down
            m_changingOsdContent = true;

            Hide(0);

            m_changingOsdContent = false;
        }

        private static void OnLateVmStart(object sender, EventArgs e)
        {
            VoicemeeterApiClient.VoicemeeterTurnedOn -= OnLateVmStart;
            var type = VoicemeeterApiClient.ProgramType;
            Application.Current.Dispatcher.Invoke(() => RefillOsd(type));
        }

        private static void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_isMouseEntered = true;
            m_displayDurationTimer.Stop();
        }

        private static void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_isMouseEntered = false;
            if (IsShown)
            {
                ResetShowTimer();
            }
        }

        private static void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsShown) return;

            Hide(75);
        }
    }
}
