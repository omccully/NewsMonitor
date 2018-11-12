using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsMonitor.WPF.Views.EditableTreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.UnitTests.WPF.Views.Controls.TreeView
{
    [TestClass]
    public class TreeModelTest
    {
        [TestMethod]
        public void Duplicate()
        {
            TreeModel<string> tm = new TreeModel<string>("",
                new List<TreeModel<string>>()
                {
                    new TreeModel<string>("section 1",
                        new List<TreeModel<string>>()
                        {
                            new TreeModel<string>("sec1item1"),
                            new TreeModel<string>("sec1item2")
                        }),
                    new TreeModel<string>("section 2"),
                });

            TreeModel<string> dupe = tm.Duplicate();
            List<TreeModel<string>> flatTm = tm.Flatten();
            List<TreeModel<string>> flatDupe = dupe.Flatten();

            Assert.IsFalse(flatTm.Any(t_Tm =>
                flatDupe.Any(t_Dupe => Object.ReferenceEquals(t_Tm, t_Dupe))));

            Assert.IsTrue(tm.Equals(dupe));

        }
    }
}
