using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Entity;
using SQLite.CodeFirst;
using NewsMonitor.Data.Models;

namespace NewsMonitor.Data.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DatabaseContext(DbConnection connection, bool contextOwnsConnection)
           : base(connection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Diagnostics.Debug.WriteLine("OnModelCreating");
            var sqliteConnectionInitializer =
                new SqliteDropCreateDatabaseWhenModelChanges<DatabaseContext>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
        }

        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<BlockedOrganization> BlockedOrganizations { get; set; }
        public DbSet<BlockedTitleMatcher> BlockedTitleMatchers { get; set; }
        public DbSet<KeyValuePair> KeyValuePairs { get; set; }
        public DbSet<SearchTerm> SearchTerms { get; set; }
    }
}
