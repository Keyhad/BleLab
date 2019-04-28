using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using InTheHand.Net.Sockets;
using Serilog;


namespace BleConsole
{
    [Verb("list", HelpText = "List all BLE devices.")]
    class CommandLineListDevices
    {
        internal static System.Threading.Tasks.Task<int> RunCommandAsync(CommandLineListDevices opts)
        {
            Log.Information("CommandLineListDevices.RunCommand");

            BluetoothClient client = new BluetoothClient();
            List<string> items = new List<string>();

            BluetoothDeviceInfo[] devices = client.DiscoverDevices(255, false, false, false, false);
            foreach (BluetoothDeviceInfo d in devices)
            {
                Log.Information("... {0}", d.DeviceName);
                items.Add(d.DeviceName);
            }

            return 0;
        }
    }
}
