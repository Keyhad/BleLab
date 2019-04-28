using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Serilog;

namespace BleDotNet
{
    [Verb("connect", HelpText = "Connect to BLE device.")]
    class CommandLineConnect
    {
        internal static object RunCommand(CommandLineConnect opts)
        {
            Log.Information("CommandLineConnect.RunCommand");
            return 0;
        }
    }
}
