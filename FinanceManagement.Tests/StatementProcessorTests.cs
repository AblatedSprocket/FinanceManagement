using System;
using System.IO;
using FinanceManagement.StatementProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinanceManagement.Tests
{
    [TestClass]
    public class StatementProcessorTests
    {
        StatementProcessor _processor = new StatementProcessor(Path.Combine("D:\\", "Homebrew", "Database", "Unprocessed"));
        [TestMethod]
        public void ProcessStatements_ProcessesAllStatements()
        {
            try
            {
                _processor.ProcessStatements();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
