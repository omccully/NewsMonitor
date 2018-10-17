using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.UnitTests.Data.Database;
using System.Linq;

namespace NewsMonitor.UnitTests.Data.Models
{
    [TestClass]
    public class NewsArticleTest
    {

        [TestMethod]
        public void NewsArticle_Adds()
        {
            const string DbFileName = "NewsArticle_Adds_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);

            dbcontext.NewsArticles.Add(
                new NewsArticle() { Title = "test title" } );
            dbcontext.SaveChanges();

            DatabaseContext dbcontext2 =
                DbTestHelpers.CreateContext(DbFileName);
            var articles = dbcontext2.NewsArticles.ToList();
            Assert.AreEqual(1, articles.Count);
            Assert.AreEqual("test title", articles.First().Title);
        }

        [TestMethod]
        public void NewsArticle_NoDuplicateUrls()
        {
            const string DbFileName = "_NewsArticle_NoDuplicates_test.sqlite";
            File.Delete(DbFileName);

            DatabaseContext dbcontext =
                DbTestHelpers.CreateContext(DbFileName);


            dbcontext.NewsArticles.Add(
                new NewsArticle(
                "Test title", "CNN", "https://cnn.com/", DateTime.MinValue));
            dbcontext.NewsArticles.Add(
                new NewsArticle(
                "Test title", "CNN", "https://cnn.com/", DateTime.Now));
            
            try
            {
                dbcontext.SaveChanges();
                Assert.Fail("Database allowed duplicate NewsArticle URLs");
            }
            catch (DbUpdateException) { }
        }


        [TestMethod]
        public void Equality_AreEqual()
        {
            DateTime dt = DateTime.Now;
            NewsArticle a = new NewsArticle(
                "Test title", "CNN", "https://cnn.com/", dt);
            NewsArticle b = new NewsArticle(
                "Test title", "CNN", "https://cnn.com/", dt);

            Assert.IsTrue(a == b);
            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(a != b);
        }

        [TestMethod]
        public void Equality_OneIsNull()
        {
            DateTime dt = DateTime.Now;
            NewsArticle a = new NewsArticle(
                "Test title", "CNN", "https://cnn.com/", dt);
            NewsArticle b = null;

            Assert.IsFalse(a == b);
            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a != b);
        }


        [TestMethod]
        public void Equality_BothNull()
        {
            NewsArticle a = null;
            NewsArticle b = null;
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
        }

        [TestMethod]
        public void Equality_TitleDifferent()
        {
            DateTime dt = DateTime.Now;
            NewsArticle a = new NewsArticle(
                "Test title1", "CNN", "https://cnn.com/", dt);
            NewsArticle b = new NewsArticle(
                "Test title2", "CNN", "https://cnn.com/", dt);

            Assert.IsTrue(a != b);
        }
    }
}
