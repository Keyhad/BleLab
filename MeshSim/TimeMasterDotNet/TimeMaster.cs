using System;

namespace TimeMasterDotNet
{
    public class TimeMaster
    {
        private long ticks;

        public TimeMaster(long offset = 0)
        {
            ticks = DateTime.Now.Ticks + offset * TimeSpan.TicksPerMillisecond;
        }

        public static long getTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void reset()
        {
            ticks = DateTime.Now.Ticks;
        }

        public bool isTimeout(long timeout)
        {
            long diff = (DateTime.Now.Ticks - ticks) / TimeSpan.TicksPerMillisecond;
            return diff > timeout;
        }

    }
}
