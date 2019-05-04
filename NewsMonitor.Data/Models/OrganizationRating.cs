using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Data.Models
{
    public class OrganizationRating
    {
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        public string OrganizationName { get; set; }

        public int Rating { get; set; }
    }
}
