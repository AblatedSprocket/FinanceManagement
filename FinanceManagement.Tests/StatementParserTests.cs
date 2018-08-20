using FinanceManagement.StatementProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinanceManagement.Tests
{
    /// <summary>
    /// Summary description for StatementParserTests
    /// </summary>
    [TestClass]
    public class StatementParserTests
    {
        StatementParser parser = new StatementParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unprocessed"));
        [TestMethod]
        public void ParseMetroStatements_ParsesAllMetroStatements()
        {
            List<Transaction> transactions = parser.ParseMetroStatementsFromCsv();
            Transaction missed = transactions.Where(t => t.Description.Contains("Home Banking")).FirstOrDefault();
            Assert.IsTrue(missed != null);
        }
    }
}