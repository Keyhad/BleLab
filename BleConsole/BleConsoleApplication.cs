using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace BleConsole
{
    class BleConsoleApplication : Application
    {
        public delegate void RunCommandDelegate(string[] args);

        public RunCommandDelegate RunCommand { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Information("BleConsoleApplication.OnStartup");

            RunCommand?.Invoke(e.Args);

            Shutdown();
        }

    }
}
