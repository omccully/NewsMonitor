using System;
using System.Collections.Generic;
using System.Text;
using NewsMonitor.Data.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using NewsMonitor.Services.NewsSearchers;

namespace NewsMonitor.Extensions.NewsSearchers.Bing
{
    public class BingNewsSearcher : INewsSearcher
    {
        const string ApiBaseUrl =
            "https://api.cognitive.microsoft.com/bing/v7.0/news/search";

        string AccessKey;

        public BingNewsSearcher(string bingNewsAccessKey)
        {
            SetAccessKey(bingNewsAccessKey);
        }

        public void SetAccessKey(string bingNewsAccessKey)
        {
            this.AccessKey = bingNewsAccessKey;
        }

        string UrlFromTerm(string term)
        {
            return $"{ApiBaseUrl}?q={Uri.EscapeDataString(term)}";
        }

        string QueryRestApi(string term)
        {
            string url = UrlFromTerm(term);
            HttpWebRequest request =
                (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", AccessKey);
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
            JObject api_result = JObject.Parse(QueryRestApi(term));

            return api_result["value"].Children().Select(
                article => BingArticleToNewsArticle(article)
                );
        }

        NewsArticle BingArticleToNewsArticle(JToken article)
        {
            string url = article.Value<string>("url");
            string title = article.Value<string>("name");

            string time_published = article.Value<string>("datePublished");
            DateTime dt_parsed = DateTime.Parse(time_published);

            string source_org = article["provider"].First.Value<string>("name");

            return new NewsArticle(title, source_org, url, dt_parsed);
        }
    }
}
