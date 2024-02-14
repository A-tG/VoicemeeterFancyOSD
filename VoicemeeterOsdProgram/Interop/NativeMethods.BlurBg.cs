using System;
using System.Runtime.InteropServices;

namespace VoicemeeterOsdProgram.Interop;

partial class NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3, 
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, // RS4 1803, MAY NOT WORK
        ACCENT_ENABLE_HOSTBACKDROP = 5, // RS5 1809, MAY NOT WORK
        ACCENT_INVALID_STATE = 6
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    [Flags]
    internal enum DWM_BB
    {
        Enable = 1,
        BlurRegion = 2,
        TransitionMaximized = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DWM_BLURBEHIND
    {
        public DWM_BB dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;

        public DWM_BLURBEHIND(bool enabled)
        {
            fEnable = enabled ? true : false;
            hRgnBlur = IntPtr.Zero;
            fTransitionOnMaximized = false;
            dwFlags = DWM_BB.Enable;
        }

        public System.Drawing.Region Region
        {
            get { return System.Drawing.Region.FromHrgn(hRgnBlur); }
        }

        public bool TransitionOnMaximized
        {
            get { return fTransitionOnMaximized; }
            set
            {
                fTransitionOnMaximized = value ? true : false;
                dwFlags |= DWM_BB.TransitionMaximized;
            }
        }

        public void SetRegion(System.Drawing.Graphics graphics, System.Drawing.Region region)
        {
            hRgnBlur = region.GetHrgn(graphics);
            dwFlags |= DWM_BB.BlurRegion;
        }
    }

    [DllImport("dwmapi.dll")]
    internal static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);
}
