using System.Windows;
using System.Windows.Controls;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for StripControl.xaml
    /// </summary>
    public partial class StripControl : UserControl
    {
        private bool m_hasChanges = false;

        public bool HasChangesFlag
        {
            get
            {
                if (m_hasChanges)
                {
                    m_hasChanges = false;
                    return true;
                }
                return m_hasChanges;
            }
            set => m_hasChanges = value;
        }

        public StripControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ShowHorizontalSeparatorAfterProperty = DependencyProperty.Register(
            "ShowHorizontalSeparatorAfter", typeof(bool), typeof(StripControl));
        public bool ShowHorizontalSeparatorAfter
        {
            get => (bool)GetValue(ShowHorizontalSeparatorAfterProperty);
            set
            {
                var thickness = BorderThickness;
                thickness.Right = value ? 1 : 0;
                BorderThickness = thickness;
                SetValue(ShowHorizontalSeparatorAfterProperty, value);
            }
        }
    }
}
