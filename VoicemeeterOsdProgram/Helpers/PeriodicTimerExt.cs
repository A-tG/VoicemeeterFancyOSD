using System;
using System.Threading;
using System.Threading.Tasks;

namespace AtgDev.Utils
{
    public class PeriodicTimerExt
    {
        private PeriodicTimer m_timer;

        public TimeSpan period;

        public void Start()
        {
            m_timer = new(period);
        }

        public void Stop()
        {
            m_timer.Dispose();
            m_timer = null;
        }

        public async ValueTask<bool> WaitForNextTickAsync() => await m_timer.WaitForNextTickAsync();
    }
}
