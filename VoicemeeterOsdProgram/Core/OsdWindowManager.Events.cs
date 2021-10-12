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

            Application.Current.Dispatcher.Invoke(() => RefillOsd(t));
        }

        private static void OnVoicemeeterLoad(object sender, EventArgs e)
        {
            var dis = Application.Current.Dispatcher;
            VoicemeeterApiClient.ProgramTypeChange += OnVoicemeeterTypeChange;
            VoicemeeterApiClient.NewParameters += (_, _) => dis.Invoke(UpdateOsd);
            VoicemeeterApiClient.VoicemeeterTurnedOff += (_, _) => dis.Invoke(OnVoicemeeterTurnedOff);
            VoicemeeterApiClient.VoicemeeterTurnedOn += (_, _) => dis.Invoke(OnVoicemeeterTurnedOn);

            if (VoicemeeterApiClient.IsVoicemeeterRunning) OnVoicemeeterTurnedOn();
        }

        private static void WaitForVoicemeeterTimerTick(object sender, EventArgs e)
        {
            UpdateVmParams(false);
            m_isWaitforVoicemeeterInit = false;
        }

        private static void OnVoicemeeterTurnedOn()
        {
            // workaround to prevent showing bugged parameters after Voicemeeter launch
            m_isWaitforVoicemeeterInit = true;
            m_WaitforVoicemeeterTimer.Stop();
            m_WaitforVoicemeeterTimer.Start();

            var type = VoicemeeterApiClient.ProgramType;
            if (type == VoicemeeterType.None) return;

            Application.Current.Dispatcher.Invoke(() => RefillOsd(VoicemeeterType.Potato));
        }

        private static void OnVoicemeeterTurnedOff()
        {
            // workaround to prevent showing bugged parameters after Voicemeeter shut down
            m_changingOsdContent = true;
            Hide(0);
            m_changingOsdContent = false;
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
