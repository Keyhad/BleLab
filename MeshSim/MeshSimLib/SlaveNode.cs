using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using TimeMasterDotNet;

namespace MeshSimLib
{
    public class SlaveNode
    {
        public const int ADVERTISING_MAX = 10;
        private Thread thread;
        public static int SamInterval { set;  get; }
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

        /// <summary>
        /// 
        /// </summary>
        public void CollectMeasurments()
        {
            //Log.Information("advertiseNode {0}", ToString());

            //if ((Id % 100) != 0 && advertisements.IsEmpty)
            if (advertisements.IsEmpty)
            {
                for (int i = 0; i < ADVERTISING_MAX; i++)
                {
                    if (i < measurements.Count)
                    {
                        try
                        {
                            MeasurementPost post;
                            if (measurements.TryDequeue(out post))
                            {
                                advertisements.Enqueue(post);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public void SlaveThread()
        {
            int startDelay = MeshSimTools.random.Next(1000, 5000);
            Thread.Sleep(startDelay);

            advertisingTimer = new TimeMaster();
            samplingTimer = new TimeMaster();

            //Log.Information("{0} starts", ToString());

            while (thread.ThreadState == ThreadState.Running)
            {
                if (samplingTimer.isTimeout(SamInterval))
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

            return ids;
        }

        public void ListenToAdvertisements(SlaveNode node, MeasurementPost[] posts)
        {
            //Log.Information("[{0}] Listen to ... ", ToString());
            foreach (MeasurementPost post in posts)
            {
                // Should listen to those position after
                if ((post.Id > this.Id) && (node.Id == this.Id+1))
                {
                    if (!measurements.Contains(post))
                    {
                        post.AdCounter++;
                        //Log.Information("... {0}", post.ToString());
                        measurements.Enqueue(post);
                    }
                }
            }

            // the node with id = 0, keep all measurements
            if (Id > 0)
            {
                while (measurements.Count > 100)
                {
                    MeasurementPost removedPost;

                    if (measurements.TryDequeue(out removedPost))
                    {
                        Log.Error("... {0} out {1}", ToString(), removedPost.ToString());
                    }
                }
            }
        }

        private void startMeasuring()
        {
            MeasurementPost post;

            int value = MeshSimTools.random.Next(0x0FFF);
            long now = samplingTimer.Now();
            post = new MeasurementPost(Id, value, now - (now % 100));
            //Log.Information("Add post {0}", post.ToString());

            Measurements.Enqueue(post);
        }

        public SlaveNode(int id)
        {
            this.Id = id;

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
            Log.Information("WaitToStop {0}", ToString());

            while (thread.IsAlive)
            {
                Thread.Sleep(100);
            }
        }

        override public string ToString()
        {
            if (advertisingTimer == null)
            {
                return string.Format("S{0:X4}, {1}", Id, Measurements.Count);
            }
            return string.Format("S{0,-4}, {1}, {2}", Id, Measurements.Count, advertisingTimer.BaseTime);
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
