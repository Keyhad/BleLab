using System;
using Serilog;
using CommandLine;
using System.Windows;
using Serilog.Context;
using System.Reflection;
using System.IO;

namespace BleConsole
{
    public class Program
    {
        private const string OutputTemplateFormat = "[{Timestamp:HH:mm:ss.fff} {Level:u3}]{Message:lj}{NewLine}{Exception}";

        [STAThread]
        public static void Main()
        {
            //String applicationLogPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            //    + "\\" + Assembly.GetExecutingAssembly().FullName + "\\log";

            string applicationLogPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\log.txt";

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Debug(outputTemplate:OutputTemplateFormat)
                            .WriteTo.Console(outputTemplate: OutputTemplateFormat)
                            .WriteTo.File(applicationLogPath, rollingInterval: RollingInterval.Day)
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

            CommandLine.Parser.Default.ParseArguments<CommandLineList, CommandLineConnect>(args)
               .MapResult(
               (CommandLineList opts) => CommandLineList.RunCommandAsync(opts),
               (CommandLineConnect opts) => CommandLineConnect.RunCommand(opts),
               errs => 1);
        }
    }

}


