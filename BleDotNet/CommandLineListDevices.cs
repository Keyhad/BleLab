using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Serilog;

namespace BleDotNet
{
    [Verb("list", HelpText = "List all BLE devices.")]
    class CommandLineListDevices
    {
        internal static int RunCommand(CommandLineListDevices opts)
        {
            Log.Information("CommandLineListDevices.RunCommand");
            return 0;
        }
    }
}
