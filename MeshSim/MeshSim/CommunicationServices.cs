using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSim
{
    public class CommunicationServices
    {
        private static readonly CommunicationServices instance = new CommunicationServices();
        public static CommunicationServices getInstance()
        {
            return instance;
        }

        private ConcurrentQueue<MeasurementPost> concurrentQueue = new ConcurrentQueue<MeasurementPost>();
        public void BroadCast(List<MeasurementPost> measurementPosts)
        {
            foreach (MeasurementPost measurementPost in measurementPosts)
            {
                concurrentQueue.Enqueue(measurementPost);
            }
        }
    }
}
