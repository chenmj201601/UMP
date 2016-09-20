using System;
using System.Diagnostics;

namespace UMPCommon
{
    public class Timing
    {
        TimeSpan duration;
        public Timing()
        {
            duration = new TimeSpan(0);
        }
        public void stopTime()
        {
            duration = Process.GetCurrentProcess().TotalProcessorTime;
        }
        public void startTime()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public TimeSpan Result()
        {
            return duration;
        }
    }
}
