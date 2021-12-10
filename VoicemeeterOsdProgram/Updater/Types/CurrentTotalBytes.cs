using System;

namespace VoicemeeterOsdProgram.Updater.Types
{
    public struct CurrentTotalBytes
    {
        private long m_current = 0;
        private long m_total = 0;

        public CurrentTotalBytes(long total)
        {
            Total = total;
        }

        public long Current
        {
            get => m_current;
            set
            {
                var val = Math.Abs(value);
                if (val > Total)
                {
                    val = Total;
                }
                m_current = val;
            }
        }

        public long Total
        { 
            get => m_total;
            set
            {
                m_total = Math.Abs(value);
            }
        }

        public double ProgressPercent
        {
            get
            {
                if (Total == 0) return 0;

                return Current * 100 / Total;
            }
        }
    }
}
