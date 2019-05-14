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
        private readonly int interval;
        private ulong id;
        private TimeMaster timeMaster;
        private NodeManager nodeManager;

        public void MasterThread()
        {
            Log.Information("MasterNode {0} starts", ToString());

            timeMaster = new TimeMaster();
            while (thread.ThreadState == ThreadState.Running)
            {
                if (timeMaster.isTimeout(interval))
                {
                    timeMaster.reset();
                    Log.Information("MasterNode {0} time to process queues", ToString());
                    foreach (SlaveNode slaveNode in nodeManager.nodesDictionary.Values)
                    {
                        foreach(ulong neighbour in slaveNode.getNeighbours())
                        {
                            SlaveNode target;
                            if (nodeManager.nodesDictionary.TryGetValue(neighbour, out target))
                            {
                                target.ListenToAdvertisements(slaveNode.AdvertisementList);
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        public MasterNode(NodeManager nodeManager, ulong id, int interval)
        {
            this.nodeManager = nodeManager;
            this.id = id;
            this.interval = interval;
            ThreadStart threadStart = new ThreadStart(MasterThread);
            thread = new Thread(threadStart);
            thread.Name = ToString();
        }

        public MasterNode(NodeManager nodeManager):this(nodeManager, 1, 3000)
        {
        }

        public void WaitToStop()
        {
            while (thread.IsAlive)
            {
                Thread.Sleep(10);
            }
        }

        public void Start()
        {
            nodeManager.Start();

            thread.Start();
        }

        public void Stop()
        {
            thread.Abort();
            WaitToStop();

            nodeManager.Stop();
            nodeManager.WaitToStop();
        }

        override public string ToString()
        {
            return string.Format("S{0:X4}", id);
        }
    }
}
