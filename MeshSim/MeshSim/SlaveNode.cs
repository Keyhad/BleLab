using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using TimeMasterDotNet;

namespace MeshSim
{
    public class SlaveNode
    {
        private Thread thread;
        private int interval;
        private ulong id;
        private TimeMaster timeMaster;

        public void SlaveThread()
        {
            Log.Information("SlaveNode {0} starts", ToString());

            Random rnd = new Random();
            long startDelay = rnd.Next(5000);

            timeMaster = new TimeMaster(startDelay);
            while (thread.ThreadState == ThreadState.Running)
            {
                if (timeMaster.isTimeout(interval))
                {
                    timeMaster.reset();
                    Log.Information("SlaveNode {0} time to ... ", ToString());
                }
                Thread.Sleep(10);
            }
        }

        public SlaveNode(ulong id, int interval)
        {
            this.id = id;
            this.interval = interval;
            ThreadStart threadStart = new ThreadStart(SlaveThread);
            thread = new Thread(threadStart);
            thread.Name = ToString();
        }

        public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            thread.Abort();
        }

        public void WaitToStop()
        {
            while (thread.IsAlive)
            {
                Thread.Sleep(10);
            }
        }

        override public string ToString()
        {
            return string.Format("S{0:X12}", id);
        }
    }
}
