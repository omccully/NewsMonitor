using System;
using System.Collections.Generic;
using System.Text;
using NewsMonitor.Data.Models;
using System.Net;
using System.IO;

namespace NewsMonitor.Services.NewsSearchers
{
    public class GoogleNewsSearcher : INewsSearcher
    {
        const string ApiBaseUrl = "https://news.google.com/news";

        string UrlFromTerm(string term)
        {
            return $"{ApiBaseUrl}?q={Uri.EscapeDataString(term)}&output=rss";
        }

        string QueryRestApi(string term)
        {
            string url = UrlFromTerm(term);
            HttpWebRequest request =
                (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                return result;
            }
        }

        public IEnumerable<NewsArticle> Search(string term)
        {
            throw new NotImplementedException();
        }
    }
}
