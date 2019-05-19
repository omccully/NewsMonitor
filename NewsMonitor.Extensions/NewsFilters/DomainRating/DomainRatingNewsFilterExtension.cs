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
    class DomainRatingNewsFilterExtension : INewsFilterExtension
    {
        public string Name => "Domain rating";

        IDomainRatingsSerializer _serializer = new JsonDomainRatingsSerializer();

        public bool AllowArticle(NewsArticle newsArticle, string searchTerm, KeyValueStorage storage)
        {
            Uri uri = new Uri(newsArticle.Url);
            string domain = String.Join(".", TakeLast(uri.Host.Split('.'),
                IsThreePartDomain(uri.Host) ? 3 : 2));

            if (_domainRatings == null) Initialize(storage);

            DomainRating rating = _domainRatings.FirstOrDefault(dr => dr.Domain == domain);
            if (rating == null)
            {
                rating = QueryDomainRating(domain);

                _domainRatings.Add(rating);
                SaveDomainRatings(storage);
            }
            else if(rating.Decision == DomainRatingDecision.Whitelisted)
            {
                return true;
            }
            else if (rating.Decision == DomainRatingDecision.Blacklisted)
            {
                return false;
            }
            else if(DateTime.Now - rating.LastChecked > TimeSpan.FromDays(31))
            {
                DomainRating tempRating = QueryDomainRating(domain);
                _domainRatings.Remove(rating);
                _domainRatings.Add(tempRating);
                SaveDomainRatings(storage);
            }

            bool allow = rating.MonthlyVisitors > storage.GetInteger(
                DomainRatingNewsFilterSettingsPage.MinimumMonthlyVisitorsKey);
            if(!allow)
            { 
                System.Diagnostics.Debug.WriteLine("Not allowing domain " +
                    domain + " " + rating);
            }
            return allow;
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

        void SaveDomainRatings(KeyValueStorage kvs)
        {
            kvs.SetValue(DomainRatingNewsFilterSettingsPage.DomainRatingsKey,
                _serializer.Serialize(_domainRatings));
        }

        List<DomainRating> _domainRatings;
        
        private void Initialize(KeyValueStorage kvs)
        {
            string serializedDomainRatings = kvs.GetString(
                DomainRatingNewsFilterSettingsPage.DomainRatingsKey);

            if (serializedDomainRatings == null)
            {
                _domainRatings = new List<DomainRating>();
            }
            else
            {
                _domainRatings = _serializer.DeserializeTyped(
                    serializedDomainRatings).Where(dr => dr.Domain != null).ToList();
            }
        }

        public Window CreateQuickFilterWindow(NewsArticle newsArticle, KeyValueStorage storage)
        {
            return null;
        }

        public SettingsPage CreateSettingsPage()
        {
            return new DomainRatingNewsFilterSettingsPage(_serializer);
        }

        DomainRating QueryDomainRating(string domainHost)
        {
            try
            {
                string queryResult = QueryRestApiAsync(domainHost);

                var doc = new HtmlDocument();
                doc.LoadHtml(queryResult);

                string innerText = doc.DocumentNode
                    .SelectNodes(@"//*[@id=""wi-monthly_visitors""]/div[2]").First().InnerText
                    .Replace(",", "");

                return new DomainRating()
                {
                    Domain = domainHost,
                    MonthlyVisitors = Int64.Parse(innerText),
                    LastChecked = DateTime.Now
                };
            }
            catch(WebException we)
            {
                HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return new DomainRating()
                    {
                        Domain = domainHost,
                        MonthlyVisitors = 0,
                        LastChecked = DateTime.Now
                    };
                }
                else
                {
                    throw;
                }
            }
        }

        string QueryRestApiAsync(string domainHost)
        {
            string domainEscaped = domainHost.Replace(".", "-");
            string url = "https://data.igoen.com/" + domainEscaped;
            HttpWebRequest request =
                (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)(request.GetResponse()))
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                return result;
            }
        }
    }
}
