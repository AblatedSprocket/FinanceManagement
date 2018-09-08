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
        StatementParser parser = new StatementParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles"));
        [TestMethod]
        public void ParseAlaskaStatementsFromCsv_ParsesStatementsSuccessfully()
        {
            //ARRANGE:
            //ACT:
            IEnumerable<Transaction> transactions = parser.ParseAlaskaStatementsFromCsv();
            //ASSERT
            Assert.IsNotNull(transactions);
        }
        [TestMethod]
        public void ParseMetroStatementsFromCsv_ParsesAllMetroStatements()
        {
            List<Transaction> transactions = parser.ParseMetroStatementsFromCsv();
            Transaction missed = transactions.Where(t => t.Description.Contains("Home Banking")).FirstOrDefault();
            Assert.IsNotNull(missed);
        }
        [TestMethod]
        public void ParseMetroStatementsFromCsv_ParsesStatementsSuccessfully()
        {
            IEnumerable<Transaction> transactions = parser.ParseMetroStatementsFromCsv();
            //ASSERT:
            Assert.IsNotNull(transactions);
        }
    }
}