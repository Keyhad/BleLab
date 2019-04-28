using System;
using Serilog;
using CommandLine;
using System.Windows;

namespace BleConsole
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();

            Log.Information("BleConsole ... Begin");

            BleConsoleApplication app = new BleConsoleApplication
            {
                RunCommand = Program.RunCommand
            };

            Window w = new Window();
            app.Run(w);

            Log.Information("BleConsole ... End");
            Log.CloseAndFlush();

        }

        private static void RunCommand(string[] args)
        {
            Log.Information("RunCommand ... {0}", String.Join(" ", args));

            CommandLine.Parser.Default.ParseArguments<CommandLineListDevices, CommandLineConnect>(args)
               .MapResult(
               (CommandLineListDevices opts) => CommandLineListDevices.RunCommandAsync(opts),
               (CommandLineConnect opts) => CommandLineConnect.RunCommand(opts),
               errs => 1);
        }
    }

}


