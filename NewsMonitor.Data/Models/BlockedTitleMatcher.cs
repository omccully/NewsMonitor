using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Data.Models
{
    public class BlockedTitleMatcher
    {
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        public string RegexText { get; set; }

        public bool CaseSensitive { get; set; }

        /// <summary>
        /// If SearchTerm is null, the block applies for any search term
        /// </summary>
        public string SearchTerm { get; set; }

        public BlockedTitleMatcher()
        {

        }

        public BlockedTitleMatcher(string regexText, bool caseSensitive=false, string searchTerm=null)
        {
            this.RegexText = regexText;
            this.CaseSensitive = caseSensitive;
            this.SearchTerm = searchTerm;
        }
    }
}
