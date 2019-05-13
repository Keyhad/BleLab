using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Serilog;

namespace MeshSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("\nMeshSim Started ... @ {0}\n", DateTime.Now);

            CommandLine.Parser.Default.ParseArguments<MeshSimOptions>(args)
                .WithParsed<MeshSimOptions>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<MeshSimOptions>((errs) => HandleParseError(errs));

            Console.ReadLine();
            Log.Information("\nMeshSim Stopped ... @ {0}\n", DateTime.Now);
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            return -1;
        }

        private static Thread mainThread;

        private static int RunOptionsAndReturnExitCode(MeshSimOptions opts)
        {
            mainThread = new Thread(startThread);
            mainThread.Name = "mainThread";
            mainThread.Start();
            return 0;
        }

        private static void startThread()
        {
            throw new NotImplementedException();
        }
    }
}
