using System;

namespace TimeMaster
{
    public class TimeMaster
    {
        private long ticks;

        public TimeMaster(long offset = 0)
        {
            ticks = offset;
        }

        public static long getTime()
        {
            return DateTime.Now.Ticks;
        }

        public void reset()
        {
            ticks = DateTime.Now.Ticks;
        }

        public bool isTimeout(long timeout)
        {
            return (DateTime.Now.Ticks - ticks) > timeout;
        }

    }
}
