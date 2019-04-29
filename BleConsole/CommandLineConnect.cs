using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Serilog;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.Background;
using Windows.Foundation;

namespace BleConsole
{
    [Verb("connect", HelpText = "Connect to BLE device.")]
    class CommandLineConnect
    {
        [Option('a', "address", Required = true, HelpText = "BLE device address.")]
        public string Address { get; set; }

        internal static int RunCommand(CommandLineConnect opts)
        {
            BluetoothLEDevice.FromIdAsync(opts.Address).Completed = (IAsyncOperation<BluetoothLEDevice> asyncInfo, AsyncStatus asyncStatus) => 
            {
                Log.Information("Completed {0:X16}", opts.Address);
            };
            //0000000B57648F72
            Log.Information("CommandLineConnect.RunCommand {0:X16}", opts.Address);
            return 0;
        }
    }
}
