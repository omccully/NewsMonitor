using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMonitor.Data.Models
{
    public class BlockedOrganization
    {
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        public string OrganizationName { get; set; }
    }
}
