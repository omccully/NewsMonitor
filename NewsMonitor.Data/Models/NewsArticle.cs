using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewsMonitor.Data.Models
{
    public class NewsArticle : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string OrganizationName { get; set; }

        [Index(IsUnique = true)]
        public string Url { get; set; }

        public DateTime TimePublished { get; set; }
        public DateTime TimeFound { get; set; }

        bool _Hidden;
        public bool Hidden {
            get
            {
                return _Hidden;
            }
            set
            {
                _Hidden = value;
                OnPropertyChanged("Hidden");
            }
        }

        private int _rating = 0;
        public int Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        private bool _userSetRating = false;
        public bool UserSetRating
        {
            get
            {
                return _userSetRating;
            }
            set
            {
                _userSetRating = value;
                OnPropertyChanged(nameof(UserSetRating));
            }
        }

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


        protected void OnPropertyChanged(string prop)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
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

        public override string ToString()
        {
            return $"{OrganizationName}: {Title}";
        }
    }
}
