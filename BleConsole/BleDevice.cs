using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace BleConsole
{
    public class BleDevice : BleObject
    {
        private BluetoothLEAdvertisementReceivedEventArgs _advertisement;

        public BleDevice(BluetoothLEAdvertisementReceivedEventArgs advertisement)
        {
            _advertisement = advertisement;
        }

        public BleDevice(string id, string name)
        {
        }

        public string Name {
            get { return _advertisement.Advertisement.LocalName; }
            set { _advertisement.Advertisement.LocalName = value; }
        }

        public short SignalStrength
        {
            get { return _advertisement.RawSignalStrengthInDBm; }
        }

        public BluetoothLEAdvertisementType AdvertisementType
        {
            get { return _advertisement.AdvertisementType; }
        }

        public ulong Id { get { return _advertisement.BluetoothAddress; } }

        public override string ToString()
        {
            return string.Format("{0:X16}, {1}DBm, {2}, {3}", Id, SignalStrength, AdvertisementType, Name);
        }
    }
}
