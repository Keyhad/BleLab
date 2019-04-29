using System;
using System.Collections.Generic;
using System.Threading;
using CommandLine;
using Serilog;
using Windows.Devices.Bluetooth.Advertisement;

namespace BleConsole
{
    [Verb("list", HelpText = "List all BLE devices.")]
    public class CommandLineList
    {
        [Option("in", Default = (short)-80, HelpText = "In range signal strength.")]
        public short? InRange { get; set; }

        [Option("out", Default = (short)-90, HelpText = "Out of range signal strength.")]
        public short? OutRange { get; set; }

        public static int RunCommandAsync(CommandLineList opts)
        {
            Log.Information("CommandLineList.RunCommand");
            BleCollection.GetInstance().Clean();

            // Create Bluetooth Listener
            var watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Only activate the watcher when we're recieving values >= -80
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = opts.InRange.GetValueOrDefault();

            // Stop watching if the value drops below -90 (user walked away)
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = opts.OutRange.GetValueOrDefault();

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();
            Thread.Sleep(10000);
            watcher.Stop();

            while (watcher.Status == BluetoothLEAdvertisementWatcherStatus.Stopping)
            {
                Thread.Sleep(1000);
            }

            BleCollection.GetInstance().Print();
            return 0;
        }

        private static void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            // Tell the user we see an advertisement and print some properties
            Log.Information("Advertisement: {0:X16}, {1}", eventArgs.BluetoothAddress, eventArgs.Advertisement.LocalName);
            BleCollection.GetInstance().AddOrUpdateDevice(new BleDevice(eventArgs));
        }
    }
}
