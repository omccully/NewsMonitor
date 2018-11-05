using NewsMonitor.Data.Database;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NewsMonitor.Data.Models;
using System.Xml.Serialization;
using System.IO;
using RedditSharp;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    public class RedditNewsSharerExtension : INewsSharerExtension
    {
        public string Name => "Reddit";

        public RedditNewsSharerExtension()
        {

        }

        public SettingsPage CreateSettingsPage()
        {
            return new RedditNewsSharerSettingsPage();
        }

        BotWebAgent botWebAgent;
        RedditSharp.Reddit RedditApi;

        string CurrentUsername;
        string CurrentPassword;
        string CurrentClientId;
        string CurrentClientSecret;

        void ReinitializeRedditApi(KeyValueStorage kvs)
        {
            System.Diagnostics.Debug.WriteLine("ReinitializeRedditApi");
            string username = kvs.GetString(RedditNewsSharerSettingsPage.RedditUsernameKey);
            string password = kvs.GetString(RedditNewsSharerSettingsPage.RedditPasswordKey);
            string clientId = kvs.GetString(RedditNewsSharerSettingsPage.RedditClientIdKey);
            string clientSecret = kvs.GetString(RedditNewsSharerSettingsPage.RedditClientSecretKey);

            if (String.IsNullOrWhiteSpace(username) ||
                String.IsNullOrWhiteSpace(password) ||
                String.IsNullOrWhiteSpace(clientId) ||
                String.IsNullOrWhiteSpace(clientSecret)) return;

            // if the user has changed the settings since the last time, it needs to be reinitialized
            if ((CurrentUsername != username || CurrentPassword != password ||
                CurrentClientId != clientId || CurrentClientSecret != clientSecret) || botWebAgent == null)
            {
                //System.Diagnostics.Debug.WriteLine($"REDDITINFO {username} {password} {clientId} {clientSecret}");
                botWebAgent = new BotWebAgent(username, password, clientId, clientSecret,
                 "https://localhost/");
                botWebAgent.UserAgent = "News Sharer (/u/EinarrPorketill)";

                RedditApi = new RedditSharp.Reddit(botWebAgent, false);

                CurrentUsername = username;
                CurrentPassword = password;
                CurrentClientId = clientId;
                CurrentClientSecret = clientSecret;
            }
        }

        public NewsSharerWindow CreateSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs)
        {
            ReinitializeRedditApi(kvs);

            RedditNewsSharerWindow sharerWindow = new RedditNewsSharerWindow(newsArticle, kvs, RedditApi);

            sharerWindow.JobsCreated += delegate (object window, JobsCreatedEventArgs e)
            {
                AddJobs(e.Jobs, kvs);
            };

            return sharerWindow;
        }

        void AddJobs(IEnumerable<IShareJob> jobs, KeyValueStorage kvs, bool updateDb = true)
        {
            foreach (IShareJob job in jobs)
            {
                job.Finished += (jobbbbb, ea) =>
                {
                    RemoveJob(job, kvs);
                };

                UnfinishedJobs.Add((RedditPostShareJob)job);
            }

            if(updateDb) UpdateUnfinishedJobsInStorage(kvs);
        }

        void RemoveJob(IShareJob job, KeyValueStorage kvs)
        {
            UnfinishedJobs.Remove((RedditPostShareJob)job);

            UpdateUnfinishedJobsInStorage(kvs);
        }

        List<RedditPostShareJob> UnfinishedJobs = new List<RedditPostShareJob>();

        public IEnumerable<IShareJob> GetUnfinishedJobs(KeyValueStorage kvs)
        {
            List<RedditPostShareJob> rawJobs = null;
            string storedXml = kvs.GetString(UnfinishedJobsKey);
            if(storedXml != null)
            {
                XmlSerializer reader = new XmlSerializer(typeof(List<RedditPostShareJob>));
                rawJobs = (List<RedditPostShareJob>)reader.Deserialize(new StringReader(storedXml));
            }

            ReinitializeRedditApi(kvs);
            foreach (RedditPostShareJob job in rawJobs)
            {
                job.RedditApi = RedditApi;
            }

            UnfinishedJobs = new List<RedditPostShareJob>();
            AddJobs(rawJobs, kvs, false);

            return rawJobs;
        }

        const string UnfinishedJobsKey = "UnfinishedJobs";

        void UpdateUnfinishedJobsInStorage(KeyValueStorage kvs)
        {
            Console.WriteLine("UpdateUnfinishedJobsInStorage Count=" + UnfinishedJobs.Count);

            XmlSerializer writer = new XmlSerializer(typeof(List<RedditPostShareJob>));
            StringWriter sw = new StringWriter();

            writer.Serialize(sw, UnfinishedJobs);

            string xml = sw.ToString();
            kvs.SetValue(UnfinishedJobsKey, xml);
            //Console.WriteLine(xml);
        }
    }
}
