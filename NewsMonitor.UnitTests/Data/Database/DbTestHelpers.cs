using NewsMonitor.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.UnitTests.Data.Database
{
    public static class DbTestHelpers
    {
        public static DatabaseContext CreateContext(string file)
        {
            //return new DatabaseContext(
            //new SQLiteConnection("data source=.\\" + file), true);
            var conn = DbProviderFactories.GetFactory("System.Data.SQLite.EF6").CreateConnection();
            conn.ConnectionString = "data source=.\\" + file;

            return null; // new DatabaseContext(conn, true);
        }
    }
}
