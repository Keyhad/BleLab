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
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("c:\\projects\\log.txt")
                .CreateLogger();

            Log.Information("\nMeshSim Started ... @ {0}\n", DateTime.Now);

            CommandLine.Parser.Default.ParseArguments<MeshSimOptions>(args)
                .WithParsed<MeshSimOptions>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<MeshSimOptions>((errs) => HandleParseError(errs));

            Log.Information("\nMeshSim Stopped ... @ {0}\n", DateTime.Now);
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            return -1;
        }

        private static int RunOptionsAndReturnExitCode(MeshSimOptions opts)
        {
            int size = opts.Size.HasValue ? opts.Size.Value : 5;
            int interval = opts.Interval.HasValue ? opts.Interval.Value : 1000;
            NodeManager nodeManager = new NodeManager(size, interval);
            MasterNode masterNode = new MasterNode(nodeManager);
            masterNode.Start();
            Console.ReadLine();
            masterNode.Stop();
            return 0;
        }

        private static void startThread()
        {
            throw new NotImplementedException();
        }
    }
}
