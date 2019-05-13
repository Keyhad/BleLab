using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMasterDotNet;

namespace MeshSim
{
    public class MeasurementPost
    {
        public ulong Id;
        int value;
        long timeStamp;

        public MeasurementPost(ulong id, int value)
        {
            this.timeStamp = TimeMaster.getTime();
            this.Id = id;
            this.value = value;
        }
    }
}
