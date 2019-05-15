using System;
using System.Collections.Concurrent;
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
        private const int ADVERTISING_INTERVAL = 1000;
        private const int REPORTING_INTERVAL = 7000;

        private Thread thread;
        private readonly int interval;
        private int id;
        public TimeMaster SimulatingTimer { get; set; }
        private TimeMaster reportingTimer;
        private NodeManager nodeManager;
        private static MasterNode instance;

        public void MasterThread()
        {
            instance = this;
            SimulatingTimer = new TimeMaster();
            reportingTimer = new TimeMaster();

            Log.Information("MasterNode {0} starts", ToString());

            while (thread.ThreadState == ThreadState.Running)
            {
                if (SimulatingTimer.isTimeout(ADVERTISING_INTERVAL))
                {
                    SimulatingTimer.reset();
                    foreach (SlaveNode slaveNode in nodeManager.Nodes)
                    {
                        slaveNode.SyncClock(SimulatingTimer.Now());
                        foreach(int neighbour in slaveNode.getNeighbours())
                        {
                            if (neighbour >= 0 && neighbour < nodeManager.Nodes.Length)
                            {
                                SlaveNode target = nodeManager.Nodes[neighbour];
                                target.ListenToAdvertisements(slaveNode.Advertisements.ToArray());
                            }
                        }
                        slaveNode.Advertisements = new ConcurrentQueue<MeasurementPost>();
                    }
                }

                if (reportingTimer.isTimeout(REPORTING_INTERVAL))
                {
                    reportingTimer.reset();
                    ReportToMaster(nodeManager.Nodes[0]);
                }

                Thread.Sleep(10);
            }
        }

        private void ReportToMaster(SlaveNode slaveNode)
        {
            //Log.Warning("ReportToMaster ... {0}", slaveNode.Measurements.Count);
#if true
            foreach (MeasurementPost post in slaveNode.Measurements)
            {
                if (!post.reported)
                {
                    Log.Warning("Measurement ... {0} ", post.ToString());
                    post.reported = true;
                }
            }
#endif
        }

        public MasterNode(NodeManager nodeManager, int id, int interval)
        {
            this.nodeManager = nodeManager;
            this.id = id;
            this.interval = interval;
            ThreadStart threadStart = new ThreadStart(MasterThread);
            thread = new Thread(threadStart);
            thread.Name = ToString();
        }

        public MasterNode(NodeManager nodeManager):this(nodeManager, 0xFFFF, 3000)
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
            return string.Format("M{0:X4}", id);
        }

        public static MasterNode GetInstance()
        {
            return instance;
        }
    }
}
