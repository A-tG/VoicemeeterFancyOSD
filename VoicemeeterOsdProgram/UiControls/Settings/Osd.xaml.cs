using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var o = OptionsStorage.Osd;
            DataContext = o;

            InitializeComponent();

            DontShowChbox.IsChecked = o.DontShowIfVoicemeeterVisible;
            IsInteractableChbox.IsChecked = o.IsInteractable;
            AnimationsChbox.IsChecked = o.AnimationsEnabled;
            WaitVmChbox.IsChecked = o.WaitForVoicemeeterInitialization;
            //AlwaysShowElements
            //NeverShowElements
            //IgnoreStripsIndexes
            //IgnoreStripsIndexes
            InitDisplayCombo();
            HorAlignmentCombo.SelectedValue = o.HorizontalAlignment;
            VertAlignmentCombo.SelectedValue = o.VerticalAlignment;

            HorAlignmentCombo.ItemsSource = HAValues;
            VertAlignmentCombo.ItemsSource = VAValues;

            o.DontShowIfVoicemeeterVisibleChanged += OptionEvent_DontShowIfVmVisible;
            o.IsInteractableChanged += OptionEvent_IsInteractableChanged;
            o.AnimationsEnabledChanged += OptionEvent_AnimationsEnabledChanged;
            o.WaitForVoicemeeterInitializationChanged += OptionEven_WaitForVmChanged;

            o.HorizontalAlignmentChanged += OptionEvent_HorAlignmentChanged;
            o.VerticalAlignmentChanged += OptionEvent_VertAlignmentChanged;

            DontShowChbox.Click += (_, _) => o.DontShowIfVoicemeeterVisible = DontShowChbox.IsChecked ?? false;
            IsInteractableChbox.Click += (_, _) => o.IsInteractable = IsInteractableChbox.IsChecked ?? false;
            AnimationsChbox.Click += (_, _) => o.AnimationsEnabled = AnimationsChbox.IsChecked ?? false;
            WaitVmChbox.Click += (_, _) => o.WaitForVoicemeeterInitialization = WaitVmChbox.IsChecked ?? false;

            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

            Unloaded += OnUnload;
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) => ReinitDisplayCombo();

        private void ReinitDisplayCombo()
        {
            OptionsStorage.Osd.DisplayIndexChanged -= OptionEvent_DisplayIndexChanged;
            DisplayCombo.SelectionChanged -= DisplayCombo_SelectionChanged;

            InitDisplayCombo();
        }

        private void InitDisplayCombo()
        {
            // Find a way to get Display's name
            var items = Enumerable.Range(0, WpfScreenHelper.Screen.AllScreens.ToList().Count).ToDictionary(i => (uint)i, i => i.ToString());
            DisplayCombo.DisplayMemberPath = "Value";
            DisplayCombo.SelectedValuePath = "Key";
            DisplayCombo.ItemsSource = items;

            var o = OptionsStorage.Osd;
            DisplayCombo.SelectedValue = o.DisplayIndex;

            o.DisplayIndexChanged += OptionEvent_DisplayIndexChanged;
            DisplayCombo.SelectionChanged += DisplayCombo_SelectionChanged;
        }

        private void DisplayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems.OfType<KeyValuePair<uint, string>>().ToArray();
            if (items.Length == 0) return;

            OptionsStorage.Osd.DisplayIndex = items[0].Key;
        }

        private void OptionEvent_DisplayIndexChanged(object sender, uint val) => DisplayCombo.SelectedValue = val;

        private void OptionEvent_VertAlignmentChanged(object sender, VertAlignment val) => VertAlignmentCombo.SelectedValue = val;

        private void OptionEvent_HorAlignmentChanged(object sender, HorAlignment val) => HorAlignmentCombo.SelectedValue = val;

        private void OptionEven_WaitForVmChanged(object sender, bool val) => WaitVmChbox.IsChecked = val;

        private void OptionEvent_AnimationsEnabledChanged(object sender, bool val) => AnimationsChbox.IsChecked = val;

        private void OptionEvent_IsInteractableChanged(object sender, bool val) => IsInteractableChbox.IsChecked = val;

        private void OptionEvent_DontShowIfVmVisible(object sender, bool val) => DontShowChbox.IsChecked = val;

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
            var o = OptionsStorage.Osd;

            o.DontShowIfVoicemeeterVisibleChanged -= OptionEvent_DontShowIfVmVisible;
            o.IsInteractableChanged -= OptionEvent_IsInteractableChanged;
            o.AnimationsEnabledChanged -= OptionEvent_AnimationsEnabledChanged;
            o.WaitForVoicemeeterInitializationChanged -= OptionEven_WaitForVmChanged;

            o.DisplayIndexChanged -= OptionEvent_DisplayIndexChanged;
            o.HorizontalAlignmentChanged -= OptionEvent_HorAlignmentChanged;
            o.VerticalAlignmentChanged -= OptionEvent_VertAlignmentChanged;
        }
    }
}
