using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMasterDotNet;

namespace MeshSimLib
{
    public class NodeManager
    {
        public int AdInterval { get; }
        public SlaveNode[] Nodes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">number of nodes</param>
        /// <param name="interval">advertising interval in ms</param>
        /// <param name="sampInterval">sampling interval in ms</param>
        public NodeManager(int size = 5, int interval = 1000, int sampInterval = 3000)
        {
            this.AdInterval = interval;
            SlaveNode.SamInterval = sampInterval;
            Nodes = new SlaveNode[size];
            for (int i = 0; i < size; i++)
            {
                Nodes[i] = new SlaveNode(i);
            }
        }

        public void Start()
        {
            foreach (SlaveNode node in Nodes)
            {
                node.Start();
            }
        }

        public void Stop()
        {
            foreach (SlaveNode node in Nodes)
            {
                node.Stop();
            }
        }

        internal void WaitToStop()
        {
            foreach (SlaveNode node in Nodes)
            {
                node.WaitToStop();
            }
        }
    }
}
