using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.UnitTests.Data.Database;
using System.Data.Entity.Infrastructure;

namespace NewsMonitor.UnitTests.Data.Models
{
    [TestClass]
    public class KeyValuePairTest
    {
        [TestMethod]
        public void KeyValuePair_Adds()
        {
            const string DbFileName = "_KeyValuePair_Adds_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.KeyValuePairs.Add(
                new KeyValuePair() { Key="testkey", Value="testvalue" });
            dbcontext.SaveChanges();

            DatabaseContext dbcontext2 =
                DbTestHelpers.CreateContext(DbFileName);
            var kvps = dbcontext2.KeyValuePairs.ToList();
            Assert.AreEqual(1, kvps.Count);
            Assert.AreEqual("testkey", kvps.First().Key);
        }

        [TestMethod]
        public void KeyValuePair_NoDuplicates()
        {
            const string DbFileName = "_KeyValuePair_NoDuplicates_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.KeyValuePairs.Add(
                new KeyValuePair() { Key = "testkey", Value = "testvalue" });
            dbcontext.KeyValuePairs.Add(
                new KeyValuePair() { Key = "testkey", Value = "testvalue2" });

            try
            {
                dbcontext.SaveChanges();
                Assert.Fail("Database allowed duplicate KeyValuePairs");
            }
            catch (DbUpdateException) { }
        }
    }
}
