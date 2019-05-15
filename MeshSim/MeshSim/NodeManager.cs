using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMasterDotNet;

namespace MeshSim
{
    public class NodeManager
    {
        public SlaveNode[] Nodes { get; }

        public NodeManager(int size = 5, int interval = 1000)
        {
            Nodes = new SlaveNode[size];
            for (int i = 0; i < size; i++)
            {
                Nodes[i] = new SlaveNode(i, interval);
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
