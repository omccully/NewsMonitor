using NewsMonitor.Data.Models;
using NewsMonitor.Services.NewsSearchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace NewsMonitor.Extensions.NewsSearchers.Google
{
    public class GoogleNewsSearcher : INewsSearcher
    {
        string TermToUrl(string term)
        {
            string escaped = Uri.EscapeDataString(term);
            return $"https://news.google.com/news?q={escaped}&output=rss";
        }

        public IEnumerable<NewsArticle> Search(string term)
        {
            string url = TermToUrl(term);
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            List<NewsArticle> articles = new List<NewsArticle>();

            foreach (SyndicationItem item in feed.Items)
            {
                if (item.Title.Text.Contains("This RSS Feed URL is deprecated"))
                    continue;

                NewsArticle article = SyndicationItemToNewsArticle(item);

                if (article == null) continue;

                articles.Add(article);
            }

            return articles;
        }

        NewsArticle SyndicationItemToNewsArticle(SyndicationItem item)
        {
            string url = ParseUrl(item.Id);
            string title = item.Title.Text;
            string organization;
            title = ParseTitle(title, out organization); 
            DateTime publishTime = item.PublishDate.UtcDateTime;

            if (url == null || title == null) return null;

            return new NewsArticle(title, organization, url, publishTime);
        }


        string ParseUrl(string id)
        {
            Regex r = new Regex("cluster=(.+)$");
            Match m = r.Match(id);

            if (m != null)
            {
                return m.Groups[1].Value;
            }

            return null;
        }

        string ParseTitle(string rssTitle, out string organization)
        {
            Regex r = new Regex("^(.+) - (.+)$");
            Match m = r.Match(rssTitle);

            if(m != null)
            {
                organization = m.Groups[2].Value;
                return m.Groups[1].Value;
            }

            organization = "";
            return rssTitle;
        }
    }
}
