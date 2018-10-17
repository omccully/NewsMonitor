using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Data.Models
{
    public class SearchTerm
    {
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        public string SearchTermText { get; set; }
    }
}
