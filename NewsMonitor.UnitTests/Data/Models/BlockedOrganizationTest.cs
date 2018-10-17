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
    public class BlockedOrganizationTest
    {
        [TestMethod]
        public void BlockedOrganization_Adds()
        {
            const string DbFileName = "_BlockedOrganization_Adds_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext = 
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.BlockedOrganizations.Add(
                new BlockedOrganization() { OrganizationName = "CNN" });
            dbcontext.SaveChanges();

            DatabaseContext dbcontext2 = 
                DbTestHelpers.CreateContext(DbFileName);
            var blocked_orgs = dbcontext2.BlockedOrganizations.ToList();
            Assert.AreEqual(1, blocked_orgs.Count);
            Assert.AreEqual("CNN", blocked_orgs.First().OrganizationName);
        }

        [TestMethod]
        public void BlockedOrganization_NoDuplicates()
        {
            const string DbFileName = "_BlockedOrganization_NoDuplicates_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext = 
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.BlockedOrganizations.Add(
                new BlockedOrganization() { OrganizationName = "Fox News" });
            dbcontext.BlockedOrganizations.Add(
                new BlockedOrganization() { OrganizationName = "Fox News" });

            try
            {
                dbcontext.SaveChanges();
                Assert.Fail("Database allowed duplicate BlockedOrganizations");
            }
            catch (DbUpdateException) { }
        }
    }
}
