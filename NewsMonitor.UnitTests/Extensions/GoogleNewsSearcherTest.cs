using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsMonitor.Extensions.NewsSearchers.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.UnitTests.Extensions
{
    [TestClass]
    public class GoogleNewsSearcherTest
    {
        [TestMethod]
        public void Test()
        {
            GoogleNewsSearcher gns = new GoogleNewsSearcher();
            var articles = gns.Search("climate change");

            Assert.IsTrue(articles.Count() > 1);
        }
    }
}
