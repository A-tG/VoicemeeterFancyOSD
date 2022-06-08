using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for Osd.xaml
    /// </summary>
    public partial class Osd : StackPanel
    {
        public Dictionary<HorAlignment, string> HAValues { get; } = new()
        {
            { HorAlignment.Left, "Left" },
            { HorAlignment.Center, "Center" },
            { HorAlignment.Right, "Right" }
        };

        public Dictionary<VertAlignment, string> VAValues { get; } = new()
        {
            { VertAlignment.Top, "Top" },
            { VertAlignment.Center, "Center" },
            { VertAlignment.Bottom, "Bottom" }
        };

        public Osd()
        {
            InitializeComponent();

            var o = OptionsStorage.Osd;

            DontShowChbox.IsChecked = o.DontShowIfVoicemeeterVisible;
            IsInteractableChbox.IsChecked = o.IsInteractable;
            ScaleSlider.Value = o.Scale;
            BgOpacitySlider.Value = o.BackgroundOpacity;
            BorderThicknessSlider.Value = o.BorderThickness;
            AnimationsChbox.IsChecked = o.AnimationsEnabled;
            WaitVmChbox.IsChecked = o.WaitForVoicemeeterInitialization;
            //AlwaysShowElements
            //NeverShowElements
            //IgnoreStripsIndexes
            //IgnoreStripsIndexes
            //DisplayIndex
            HorAlignmentCombo.SelectedValue = o.HorizontalAlignment;
            VertAlignmentCombo.SelectedValue = o.VerticalAlignment;

            o.DontShowIfVoicemeeterVisibleChanged += OptionEvent_DontShowIfVmVisible;
            o.IsInteractableChanged += OptionEvent_IsInteractableChanged;
            o.ScaleChanged += OptionEvent_ScaleChanged;
            o.BackgroundOpacityChanged += OptionEvent_BgOpacityChanged;
            o.BorderThicknessChanged += OptionEvent_BorderThicknessChanged;
            o.AnimationsEnabledChanged += OptionEvent_AnimationsEnabledChanged;
            o.WaitForVoicemeeterInitializationChanged += OptionEven_WaitForVmChanged;

            o.HorizontalAlignmentChanged += OptionEvent_HorAlignmentChanged;
            o.VerticalAlignmentChanged += OptionEvent_VertAlignmentChanged;

            Unloaded += OnUnload;
        }

        private void OptionEvent_VertAlignmentChanged(object sender, VertAlignment val) => VertAlignmentCombo.SelectedValue = val;

        private void OptionEvent_HorAlignmentChanged(object sender, HorAlignment val) => HorAlignmentCombo.SelectedValue = val;

        private void OptionEven_WaitForVmChanged(object sender, bool val) => WaitVmChbox.IsChecked = val;

        private void OptionEvent_AnimationsEnabledChanged(object sender, bool val) => AnimationsChbox.IsChecked = val;

        private void OptionEvent_BorderThicknessChanged(object sender, double val) => BorderThicknessSlider.Value = val;

        private void OptionEvent_ScaleChanged(object sender, double val) => ScaleSlider.Value = val;

        private void OptionEvent_BgOpacityChanged(object sender, double val) => BgOpacitySlider.Value = val;

        private void OptionEvent_IsInteractableChanged(object sender, bool val) => IsInteractableChbox.IsChecked = val;

        private void OptionEvent_DontShowIfVmVisible(object sender, bool val) => DontShowChbox.IsChecked = val;

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            var o = OptionsStorage.Osd;

            o.DontShowIfVoicemeeterVisibleChanged -= OptionEvent_DontShowIfVmVisible;
            o.IsInteractableChanged -= OptionEvent_IsInteractableChanged;
            o.ScaleChanged -= OptionEvent_ScaleChanged;
            o.BackgroundOpacityChanged -= OptionEvent_BgOpacityChanged;
            o.BorderThicknessChanged -= OptionEvent_BorderThicknessChanged;
            o.AnimationsEnabledChanged -= OptionEvent_AnimationsEnabledChanged;
            o.WaitForVoicemeeterInitializationChanged -= OptionEven_WaitForVmChanged;

            o.HorizontalAlignmentChanged -= OptionEvent_HorAlignmentChanged;
            o.VerticalAlignmentChanged -= OptionEvent_VertAlignmentChanged;
        }
    }
}
