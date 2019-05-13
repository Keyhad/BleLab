using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace MeshSim
{
    public class SlaveNode
    {
        private Thread thread;
        private int interval;
        private ulong id;

        public void SlaveThread()
        {
            Log.Information("SlaveNode {0} starts", ToString());

            DateTime.Now.Ticks
            while(thread.ThreadState == ThreadState.Running)
            {
                Thread.Sleep(interval);
            }
        }

        public SlaveNode(ulong id, int interval)
        {
            this.id = id;
            ThreadStart threadStart = new ThreadStart(SlaveThread);
            thread = new Thread(threadStart);
            thread.Name = ToString();
            thread.Start();
        }

        override public string ToString()
        {
            return string.Format("S{0:12X}", id);
        }
    }
}
