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

        public delegate void ListenToAdvertisements();
        ListenToAdvertisements listenToAdvertisements;

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

        public void SlaveThread()
        {
            Log.Information("SlaveNode {0} starts", ToString());

            while (thread.ThreadState == ThreadState.Running)
            {
                if (timeMaster.isTimeout(interval))
                {
                    timeMaster.reset();
                    Log.Information("SlaveNode {0} time to ... ", ToString());

                    startMeasuring();
                    advertiseNode();

                    listenToAdvertisements?.Invoke();
                }
                Thread.Sleep(10);
            }
        }

        private void listenToAdvertises()
        {

        }

        private void advertiseNode()
        {
            if (advertisementList.Count > 5) {
                advertisementList.Count
            }
        }

        private void startMeasuring()
        {
            Random rnd = new Random();
            int value = rnd.Next(0x0FFF);
            measurementsList.Add(new MeasurementPost(id, value));
        }

        public SlaveNode(ulong id, int interval)
        {
            this.id = id;
            this.interval = interval;

            Random rnd = new Random();
            long startDelay = rnd.Next(5000);
            timeMaster = new TimeMaster(startDelay);

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
            return string.Format("S{0:X12}", id);
        }
    }
}
