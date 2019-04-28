using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Serilog;

namespace BleConsole
{
    [Verb("connect", HelpText = "Connect to BLE device.")]
    class CommandLineConnect
    {
        internal static int RunCommand(CommandLineConnect opts)
        {
            Log.Information("CommandLineConnect.RunCommand");
            return 0;
        }
    }
}
