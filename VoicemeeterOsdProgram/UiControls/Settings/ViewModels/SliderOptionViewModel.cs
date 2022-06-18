using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels
{
    public class SliderOptionViewModel : BaseViewModel
    {
        private string m_label = "";
        private double m_min, m_max, m_tickFreq, m_smallChange, m_largeChange;
        
        public string Label
        {
            get => m_label;
            set
            {
                m_label = value;
                OnPropertyChanged();
            }
        }

        public double Min
        {
            get => m_min;
            set
            {
                m_min = value;
                OnPropertyChanged();
            }
        }

        public double Max
        {
            get => m_max;
            set
            {
                m_max = value;
                OnPropertyChanged();
            }
        }

        public double TickFreq
        {
            get => m_tickFreq;
            set 
            { 
                m_tickFreq = value; 
                OnPropertyChanged(); 
            } 
        }

        public double SmallChange
        {
            get => m_smallChange;
            set
            {
                m_smallChange = value;
                OnPropertyChanged();
            }
        }

        public double LargeChange
        {
            get => m_largeChange;
            set
            {
                m_largeChange = value;
                OnPropertyChanged();
            }
        }
    }
}
