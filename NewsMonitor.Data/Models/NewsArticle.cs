using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewsMonitor.Data.Models
{
    public class NewsArticle
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string OrganizationName { get; set; }

        [Index(IsUnique=true)]
        public string Url { get; set; }

        public DateTime TimePublished { get; set; }
        public DateTime TimeFound { get; set; }
        public bool Hidden { get; set; }

        public NewsArticle()
        {

        }

        public NewsArticle(string title, string organization, string url,
            DateTime timePublished)
        {
            this.Title = title;
            this.OrganizationName = organization;
            this.Url = url;
            this.TimePublished = timePublished;

            this.TimeFound = DateTime.Now;
            this.Hidden = false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Routine return false.
            NewsArticle r = obj as NewsArticle;

            return this.Equals(r);
        }

        public bool Equals(NewsArticle lc)
        {
            if ((Object)lc == null)
            {
                return false;
            }

            return this.Title == lc.Title &&
                this.OrganizationName == lc.OrganizationName &&
                this.Url == lc.Url &&
                this.TimePublished == lc.TimePublished;
        }

        public static bool operator ==(NewsArticle a, NewsArticle b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(NewsArticle a, NewsArticle b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode() ^ OrganizationName.GetHashCode()
                ^ Url.GetHashCode() ^ TimePublished.GetHashCode();
        }
    }
}
