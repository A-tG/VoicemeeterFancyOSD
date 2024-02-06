namespace VoicemeeterOsdProgram.Updater.Types;

public struct CurrentTotalBytes
{
    private ulong m_current = 0;
    private ulong m_total = 0;

    public CurrentTotalBytes(ulong total)
    {
        Total = total;
    }

    public ulong Current
    {
        get => m_current;
        set
        {
            var val = value;
            if (val > Total)
            {
                val = Total;
            }
            m_current = val;
        }
    }

    public ulong Total
    { 
        get => m_total;
        set
        {
            m_total = value;
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
