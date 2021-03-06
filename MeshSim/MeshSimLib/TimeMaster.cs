﻿using System;

namespace TimeMasterDotNet
{
    public class TimeMaster
    {
        /// <summary>
        /// ticks
        /// </summary>
        private long ticks0;
        /// <summary>
        /// ticks in ms based on RealTime
        /// </summary>
        private long ticks_ms;
        /// <summary>
        /// Base time in ms, keeps a refrence as a base-time
        /// </summary>
        public long BaseTime { get; set; }

        public TimeMaster(long offset = 0)
        {
            ticks0 = DateTime.Now.Ticks;
            reset(offset);
            BaseTime = Now0();
        }

        public void reset(long offset = 0)
        {
            ticks_ms = Now() + offset;
        }

        public bool isTimeout(int timeout)
        {
            long diff = Now() - ticks_ms;
            return diff > timeout;
        }

        public long Now0()
        {
            return (DateTime.Now.Ticks - ticks0) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Now in ms
        /// </summary>
        /// <returns></returns>
        public long Now()
        {
            long diff = Now0() - BaseTime;
            return diff;
        }

    }
}
