using HtmlAgilityPack;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    class DomainRatingNewsFilterExtension : INewsFilterExtension, IDomainRaterFactory
    {
        public string Name => "Domain rating";

        DomainRater _domainRater;
        IDomainRatingsSerializer _serializer = new JsonDomainRatingsSerializer();

        public bool AllowArticle(NewsArticle newsArticle, string searchTerm, KeyValueStorage storage)
        {
            Uri uri = new Uri(newsArticle.Url);
            string domain = String.Join(".", TakeLast(uri.Host.Split('.'),
                IsThreePartDomain(uri.Host) ? 3 : 2));

            InitializeDomainRater(storage);

            int minimumMonthlyVisitors = storage.GetInteger(
                DomainRatingNewsFilterSettingsPage.MinimumMonthlyVisitorsKey);

            if (minimumMonthlyVisitors <= 0) return true;

            DomainRating rating = _domainRater.GetDomainRating(domain);

            if (rating.Decision == DomainRatingDecision.Whitelisted)
            {
                return true;
            }
            else if (rating.Decision == DomainRatingDecision.Blacklisted)
            {
                return false;
            }

            bool allow = rating.MonthlyVisitors >= minimumMonthlyVisitors;
            if(!allow)
            { 
                System.Diagnostics.Debug.WriteLine("Not allowing domain " +
                    domain + " " + rating);
            }
            return allow;
        }

        void InitializeDomainRater(KeyValueStorage kvs)
        {
            if(_domainRater == null)
            {
                _domainRater = new DomainRater(kvs, _serializer);
            }
        }

        bool IsThreePartDomain(string domain)
        {
            Regex regex = new Regex(@"\bcom?.\w+$");
            return regex.IsMatch(domain);
        }

        public static IEnumerable<T> TakeLast<T>(IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        
        public Window CreateQuickFilterWindow(NewsArticle newsArticle, KeyValueStorage storage)
        {
            return null;
        }

        public SettingsPage CreateSettingsPage()
        {
            return new DomainRatingNewsFilterSettingsPage(_serializer);
        }
       
        public IDomainRater CreateDomainRater(KeyValueStorage kvs)
        {
            InitializeDomainRater(kvs);
            return _domainRater;
        }
    }
}
