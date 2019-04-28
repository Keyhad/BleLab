using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using Serilog;
using Windows.Devices.Bluetooth.Advertisement;

namespace BleConsole
{
    [Verb("list", HelpText = "List all BLE devices.")]
    class CommandLineListDevices
    {
        internal static int RunCommandAsync(CommandLineListDevices opts)
        {
            Log.Information("CommandLineListDevices.RunCommand");

            // Create Bluetooth Listener
            var watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Only activate the watcher when we're recieving values >= -80
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // Stop watching if the value drops below -90 (user walked away)
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();

            Thread.Sleep(10000);
            return 0;
        }

        private static void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            // Tell the user we see an advertisement and print some properties
            Log.Information("Advertisement: {0}, {1}", eventArgs.BluetoothAddress, eventArgs.Advertisement.LocalName);
        }
    }
}
