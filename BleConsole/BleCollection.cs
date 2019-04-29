using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BleConsole
{
    public class BleCollection
    {
        private static BleCollection _bleCollection;

        private Dictionary<ulong, BleDevice> _devices = new Dictionary<ulong, BleDevice>();

        public static BleCollection GetInstance()
        {
            if (_bleCollection == null)
            {
                _bleCollection = new BleCollection();
            }
            return _bleCollection;
        }

        public BleCollection()
        {
        }

        public void Clean()
        {
            _devices.Clear();
        }

        public void AddOrUpdateDevice(BleDevice device)
        {
            if (_devices.ContainsKey(device.Id))
            {
                BleDevice bd = _devices[device.Id];
                // update/complete info
                if (!string.IsNullOrEmpty(device.Name))
                {
                    bd.Name = device.Name;
                }
            }
            else
            {
                _devices[device.Id] = device;
            }
        }

        internal void Print()
        {
            Log.Information("Number of devices found: {0}", _devices.Count);
            foreach (KeyValuePair<ulong, BleDevice> bd in _devices)
            {
                Log.Information("Print: {0}, {1}", bd.Value.Id, bd.Value.Name);
            }
        }
    }
}
