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
    public class SlaveNode
    {
        private const int SAMPLING_INTERVAL = 10000;
        private const int ADVERTISING_INTERVAL = 1700;
        private Thread thread;
        private int interval;
        public int Id;
        private TimeMaster advertisingTimer;
        private TimeMaster samplingTimer;

        private ConcurrentQueue<MeasurementPost> measurements = new ConcurrentQueue<MeasurementPost>();
        private ConcurrentQueue<MeasurementPost> advertisements = new ConcurrentQueue<MeasurementPost>();

        /// <summary>
        /// Position on x axis
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Position on y axis
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Position on z axis
        /// </summary>
        public int Z { get; set; }
        /// <summary>
        /// list of measurement for advertising
        /// </summary>
        public ConcurrentQueue<MeasurementPost> Advertisements { get => advertisements; set => advertisements = value; }
        /// <summary>
        /// list of all measurements
        /// </summary>
        public ConcurrentQueue<MeasurementPost> Measurements { get => measurements; set => measurements = value; }

        public void SlaveThread()
        {
            int startDelay = MeshSimTools.random.Next(1000, 5000);
            Thread.Sleep(startDelay);

            advertisingTimer = new TimeMaster();
            samplingTimer = new TimeMaster();

            Log.Information("{0} starts", ToString());

            while (thread.ThreadState == ThreadState.Running)
            {
                if (advertisingTimer.isTimeout(ADVERTISING_INTERVAL))
                {
                    advertisingTimer.reset();
                    advertiseNode();
                }

                if (samplingTimer.isTimeout(SAMPLING_INTERVAL))
                {
                    samplingTimer.reset();
                    startMeasuring();
                }
                Thread.Sleep(10);
            }
        }

        internal IEnumerable<int> getNeighbours()
        {
            List<int> ids = new List<int>();

            ids.Add(Id - 2);
            ids.Add(Id - 1);
            ids.Add(Id + 1);
            ids.Add(Id + 2);
            ids.Add(Id + 3);

            return ids;
        }

        public void ListenToAdvertisements(MeasurementPost[] posts)
        {
            //Log.Information("[{0}] Listen to ... ", ToString());
            foreach (MeasurementPost post in posts)
            {
                // Should listen to those position after
                if (post.Id > this.Id)
                {
                    //Log.Information("... {0}", post.ToString());
                    if (!measurements.Contains(post))
                    {
                        measurements.Enqueue(post);
                    }
                }
            }

            // the node with id = 0, keep all measurements
            if (Id > 0)
            {
                lock (measurements)
                {
                    while (measurements.Count > 100)
                    {
                        MeasurementPost removedPost;
                        if (measurements.TryDequeue(out removedPost))
                        {

                        }
                    }
                }
            }
        }

        private void advertiseNode()
        {
            //Log.Information("advertiseNode {0}", ToString());

            if (advertisements.IsEmpty)
            { 
                for (int i = 0; i < 5; i++)
                {
                    if (i < measurements.Count)
                    {
                        int index = MeshSimTools.random.Next(0, measurements.Count);
                        advertisements.Enqueue(measurements.ElementAt(index));
                    }
                }
            }
        }

        private void startMeasuring()
        {
            int value = MeshSimTools.random.Next(0x0FFF);
            long now = samplingTimer.Now();
            MeasurementPost post = new MeasurementPost(Id, value, now - (now % 100));
            post.MainTimeStamp = MasterNode.GetInstance().SimulatingTimer.Now();
            //Log.Information("Add post {0}", post.ToString());

            Measurements.Enqueue(post);
        }

        public SlaveNode(int id, int interval)
        {
            this.Id = id;
            this.interval = interval;

            X = (int)id;
            Y = 0;
            Z = 0;

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
            if (advertisingTimer == null)
            {
                return string.Format("S{0:X4}, {1}", Id, Measurements.Count);
            }
            return string.Format("S{0:X4}, {1}, {2}", Id, Measurements.Count, advertisingTimer.BaseTime);
        }

        public void SyncClock(long now)
        {
            if (samplingTimer != null)
            {
                samplingTimer.BaseTime = samplingTimer.Now() - now;
            }
        }
    }
}
