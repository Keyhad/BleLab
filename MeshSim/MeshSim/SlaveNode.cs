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
        public ulong Id;
        private TimeMaster timeMaster;

        List<MeasurementPost> measurementsList = new List<MeasurementPost>();
        List<MeasurementPost> advertisementList = new List<MeasurementPost>();

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
        public List<MeasurementPost> AdvertisementList { get => advertisementList; set => advertisementList = value; }

        public void SlaveThread()
        {
            int startDelay = MeshSimTools.random.Next(5000);
            Thread.Sleep(startDelay);

            Log.Information("SlaveNode {0} starts", ToString());

            while (thread.ThreadState == ThreadState.Running)
            {
                if (timeMaster.isTimeout(interval))
                {
                    timeMaster.reset();
                    Log.Information("SlaveNode {0} time to ... ", ToString());

                    startMeasuring();
                    advertiseNode();

                    //ListenToAdvertisementsEvent(this.advertisementList);
                }
                Thread.Sleep(10);
            }
        }

        internal IEnumerable<ulong> getNeighbours()
        {
            List<ulong> ids = new List<ulong>();

            ids.Add(Id - 2);
            ids.Add(Id - 1);
            ids.Add(Id + 1);
            ids.Add(Id + 2);
            ids.Add(Id + 3);

            return ids;
        }

        public void ListenToAdvertisements(List<MeasurementPost> advertisementList)
        {
            foreach (MeasurementPost post in advertisementList)
            {
                if (post.Id != this.Id)
                {
                    measurementsList.Add(post);
                }
            }

            while (measurementsList.Count > 10)
            {
                measurementsList.RemoveAt(0);
            }
        }

        private void advertiseNode()
        {
            if (AdvertisementList.Count > 5) {
            }
        }

        private void startMeasuring()
        {
            int value = MeshSimTools.random.Next(0x0FFF);
            measurementsList.Add(new MeasurementPost(Id, value));
        }

        public SlaveNode(ulong id, int interval)
        {
            this.Id = id;
            this.interval = interval;
            timeMaster = new TimeMaster();

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
            return string.Format("S{0:X12}, {1}", Id, measurementsList.Count);
        }
    }
}
