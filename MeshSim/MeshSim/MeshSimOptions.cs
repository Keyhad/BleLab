using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace MeshSim
{
    class MeshSimOptions
    {
        [Option('r', "read",
            HelpText = "Input files to be processed.")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('v', "verbose",
          Default = false,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option("stdin",
          Default = false,
          HelpText = "Read from stdin")]
        public bool stdin { get; set; }

        [Value(0, 
            MetaName = "size", 
            Default = 100,
            HelpText = "Number of nodes.")]
        public int? Size { get; set; }

        [Value(1,
            MetaName = "adInterval",
            Default = 5000,
            HelpText = "Advertising interval in ms.")]
        public int? AdInterval { get; set; }

        [Value(2,
            MetaName = "samInterval",
            Default = 10000,
            HelpText = "Sampling interval in ms.")]
        public int? SamInterval { get; set; }

    }
}
