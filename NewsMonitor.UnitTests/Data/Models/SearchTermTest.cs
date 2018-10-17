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
    public class SearchTermTest
    {
        [TestMethod]
        public void SearchTerm_Adds()
        {
            const string DbFileName = "_SearchTerm_Adds_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.SearchTerms.Add(
                new SearchTerm() { SearchTermText="climate change" });
            dbcontext.SaveChanges();

            DatabaseContext dbcontext2 =
                DbTestHelpers.CreateContext(DbFileName);
            var search_terms = dbcontext2.SearchTerms.ToList();
            Assert.AreEqual(1, search_terms.Count);
            Assert.AreEqual("climate change", search_terms.First().SearchTermText);
        }

        [TestMethod]
        public void SearchTerm_NoDuplicate()
        {
            const string DbFileName = "_SearchTerm_NoDuplicate_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.SearchTerms.Add(
                new SearchTerm() { SearchTermText = "climate change" });
            dbcontext.SearchTerms.Add(
                new SearchTerm() { SearchTermText = "climate change" });

            try
            {
                dbcontext.SaveChanges();
                Assert.Fail("Database allowed duplicate SearchTerms");
            }
            catch (DbUpdateException) { }
        }

    }
}
