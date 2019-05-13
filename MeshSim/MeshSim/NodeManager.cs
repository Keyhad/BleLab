using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSim
{
    public class NodeManager
    {
        public readonly Dictionary<ulong, SlaveNode> nodesDictionary = new Dictionary<ulong, SlaveNode>();

        public NodeManager(int size = 5, int interval = 1000)
        {
            for (int i = 0; i < size; i++)
            {
                ulong id = (ulong)(0x1000 + i);
                nodesDictionary[id] = new SlaveNode(id, interval);
            }
        }

        public void Start()
        {
            foreach (SlaveNode node in nodesDictionary.Values)
            {
                node.Start();
            }
        }

        public void Stop()
        {
            foreach (SlaveNode node in nodesDictionary.Values)
            {
                node.Stop();
            }
        }

        internal void WaitToStop()
        {
            foreach (SlaveNode node in nodesDictionary.Values)
            {
                node.WaitToStop();
            }
        }
    }
}
