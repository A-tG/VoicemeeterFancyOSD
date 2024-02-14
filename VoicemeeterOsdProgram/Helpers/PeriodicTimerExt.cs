using System;
using System.Threading;
using System.Threading.Tasks;

namespace AtgDev.Utils;

public class PeriodicTimerExt
{
    private PeriodicTimer m_timer;

    public TimeSpan period;

    public PeriodicTimerExt(TimeSpan period) => this.period = period;

    public void Start()
    {
        Stop();
        m_timer = new(period);
    }

    public void Stop()
    {
        m_timer?.Dispose();
    }

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken token = default)
    {
        if (m_timer is null) return false;

        return await m_timer.WaitForNextTickAsync(token);
    }
}
