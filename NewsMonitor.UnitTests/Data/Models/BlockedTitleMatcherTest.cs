using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.UnitTests.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.UnitTests.Data.Models
{
    [TestClass]
    public class BlockedTitleMatcherTest
    {
        [TestMethod]
        public void BlockedTitleMatcher_Adds()
        {
            const string DbFileName = "_BlockedTitleMatcher_Adds_test.sqlite";
            File.Delete(DbFileName);

            string regex = "Cops:";

            DatabaseContext dbcontext = 
                DbTestHelpers.CreateContext(DbFileName);
            dbcontext.BlockedTitleMatchers.Add(
                new BlockedTitleMatcher(regex));
            dbcontext.SaveChanges();


            DatabaseContext dbcontext2 = 
                DbTestHelpers.CreateContext(DbFileName);
            var blocked_title_matchers = dbcontext2.BlockedTitleMatchers.ToList();

            Assert.AreEqual(1, blocked_title_matchers.Count);
            Assert.AreEqual(regex, blocked_title_matchers.First().RegexText);
        }

        [TestMethod]
        public void BlockedTitleMatcher_NoDuplicates()
        {
            const string DbFileName = "_BlockedTitleMatcher_NoDuplicates_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.BlockedTitleMatchers.Add(
                new BlockedTitleMatcher("indicted"));
            dbcontext.BlockedTitleMatchers.Add(
                new BlockedTitleMatcher("indicted"));
            try
            {
                dbcontext.SaveChanges();
                Assert.Fail("Database allowed duplicate BlockedTitleMatchers");
            }
            catch (DbUpdateException) { }
        }
    }
}
