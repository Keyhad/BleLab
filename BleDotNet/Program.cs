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

            Console.WriteLine("Hello World!");

            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
               .WithParsed<CommandLineOptions>(opts => RunOptionsAndReturnExitCode(opts))
               .WithNotParsed<CommandLineOptions>((errs) => HandleParseError(errs));

            Log.CloseAndFlush();
            Console.ReadKey();
        }
    }
}
