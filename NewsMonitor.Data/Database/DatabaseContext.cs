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
    public class DatabaseContext : SQLiteDB.SqLiteDbContext<DatabaseContext>
    {
        private const int CurrentSchemaVersion = 2;

        public DatabaseContext() : base("name=NewsMonitorDb", CurrentSchemaVersion) 
        {
        }
        
        public DatabaseContext(string nameOrConnectionString)
            : base(nameOrConnectionString, CurrentSchemaVersion)
        {
        }

        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<BlockedOrganization> BlockedOrganizations { get; set; }
        public DbSet<BlockedTitleMatcher> BlockedTitleMatchers { get; set; }
        public DbSet<KeyValuePair> KeyValuePairs { get; set; }
        public DbSet<SearchTerm> SearchTerms { get; set; }
        public DbSet<ShareJobResult> ShareJobResults { get; set; }
        public DbSet<OrganizationRating> OrganizationRatings { get; set; }
    }
}
