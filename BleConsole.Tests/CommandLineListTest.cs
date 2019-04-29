using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BleConsole;

namespace BleConsole.Tests
{
    [TestClass]
    public class CommandLineListTest
    {
        [TestMethod]
        public void RunCommandAsyncTest()
        {
            int exitCode = 0;

            // Prepare

            // Execute
            exitCode = CommandLineList.RunCommandAsync(null) ;

            // Verify
            Assert.IsTrue(exitCode == 0);
        }
    }
}
