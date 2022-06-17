using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for OsdElementsListView.xaml
    /// </summary>
    public partial class StripElementsListView
    {
        public StripElementsListView()
        {
            InitializeComponent();
            ListViewControl.SelectionChanged += ListViewControl_SelectionChanged;
        }

        private void ListViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems), typeof(IEnumerable), typeof(StripElementsListView), new PropertyMetadata(new HashSet<object>()));

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }
    }
}
