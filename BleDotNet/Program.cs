using System;
using Serilog;
using CommandLine;

namespace BleDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();

            Log.Information("Hello, world!");

            CommandLine.Parser.Default.ParseArguments<CommandLineListDevices, CommandLineConnect>(args)
               .MapResult(
               (CommandLineListDevices opts) => CommandLineListDevices.RunCommand(opts),
               (CommandLineConnect opts) => CommandLineConnect.RunCommand(opts),
               errs => 1);

            Log.CloseAndFlush();
            Console.ReadKey();
        }
    }
}
