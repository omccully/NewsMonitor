using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Data.Models
{
    public class ShareJobResult
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool Skipped { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime Time { get; set; }

        public ShareJobResult()
        {

        }

        public ShareJobResult(ShareJobResult other)
        {
            Id = other.Id;
            Description = other.Description;
            Url = other.Url;
            Skipped = other.Skipped;
            ErrorMessage = other.ErrorMessage;
            Time = other.Time;
        }

        public ShareJobResult(string description, string url, bool skipped = false, string errorMessage = null)
        {
            this.Description = description;
            this.Url = url;
            this.Skipped = skipped;
            this.ErrorMessage = errorMessage;
            this.Time = DateTime.Now;
        }
    }
}
