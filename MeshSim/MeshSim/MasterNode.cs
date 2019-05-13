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
    public class MasterNode
    {
        private Thread thread;
        private int interval;
        private ulong id;
        private TimeMaster timeMaster;

        public void SlaveThread()
        {
            Log.Information("MasterNode {0} starts", ToString());

            timeMaster = new TimeMaster();
            while (thread.ThreadState == ThreadState.Running)
            {
                if (timeMaster.isTimeout(interval))
                {
                    Log.Information("MasterNode {0} time to process queues", ToString());
                }
            }
        }

        public MasterNode(ulong id, int interval)
        {
            this.id = id;
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

        override public string ToString()
        {
            return string.Format("S{0:12X}", id);
        }
    }
}
