using HtmlAgilityPack;
using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NewsMonitor.Extensions.NewsFilters.DomainRating
{
    class DomainRater : IDomainRater
    {
        List<DomainRating> _domainRatings;
        KeyValueStorage _kvs;
        IDomainRatingsSerializer _serializer;

        public DomainRater(KeyValueStorage kvs, IDomainRatingsSerializer serializer)
        {
            _kvs = kvs;
            _serializer = serializer;

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
        void SaveDomainRatings(KeyValueStorage kvs)
        {
            kvs.SetValue(DomainRatingNewsFilterSettingsPage.DomainRatingsKey,
                _serializer.Serialize(_domainRatings));
        }

        public DomainRating GetDomainRating(string domain)
        {
            DomainRating rating = _domainRatings.FirstOrDefault(dr => dr.Domain == domain);

            if(rating == null)
            {
                rating = QueryDomainRating(domain);

                _domainRatings.Add(rating);
                SaveDomainRatings(_kvs);
            }
            else if(DateTime.Now - rating.LastChecked > TimeSpan.FromDays(31))
            {
                DomainRating tempRating = QueryDomainRating(domain);
                _domainRatings.Remove(rating);
                _domainRatings.Add(tempRating);
                SaveDomainRatings(_kvs);
                rating = tempRating;
            }

            return rating;
        }

        public long GetMonthlyVisitors(string domain)
        {
            return GetDomainRating(domain).MonthlyVisitors;
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

                long monthlyVisitors;
                bool succuess = Int64.TryParse(innerText, out monthlyVisitors);

                return new DomainRating()
                {
                    Domain = domainHost,
                    MonthlyVisitors = succuess ? monthlyVisitors : 0,
                    LastChecked = DateTime.Now
                };
            }
            catch (WebException we)
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
