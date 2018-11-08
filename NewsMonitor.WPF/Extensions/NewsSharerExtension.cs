using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.WPF.Views;

namespace NewsMonitor.WPF.Extensions
{
    public abstract class NewsSharerExtension<T> : INewsSharerExtension where T : IShareJob
    {
        public abstract string Name { get; }

        public abstract SettingsPage CreateSettingsPage();

        protected void BeginListenForJobsFromSharerWindow(NewsSharerWindow nsw, KeyValueStorage kvs)
        {
            nsw.JobsCreated += delegate (object window, JobsCreatedEventArgs e)
            {
                AddJobs(e.Jobs.Cast<T>(), kvs);
            };
        }

        protected List<T> UnfinishedJobs = new List<T>();


        void AddJobs(IEnumerable<T> jobs, KeyValueStorage kvs, bool updateDb = true)
        {
            foreach (T job in jobs)
            {
                job.Finished += (jobbbbb, ea) =>
                {
                    RemoveJob(job, kvs);
                };

                UnfinishedJobs.Add(job);
            }

            if (updateDb) UpdateUnfinishedJobsInStorage(kvs);
        }

        void RemoveJob(T job, KeyValueStorage kvs)
        {
            UnfinishedJobs.Remove(job);

            UpdateUnfinishedJobsInStorage(kvs);
        }

        const string UnfinishedJobsKey = "UnfinishedJobs";

        void UpdateUnfinishedJobsInStorage(KeyValueStorage kvs)
        {
            Console.WriteLine("UpdateUnfinishedJobsInStorage Count=" + UnfinishedJobs.Count);

            kvs.SetValue(UnfinishedJobsKey, SerializeShareJobs(UnfinishedJobs));
        }

        protected virtual string SerializeShareJobs(List<T> jobs)
        {
            XmlSerializer writer = new XmlSerializer(typeof(List<T>));
            StringWriter sw = new StringWriter();

            writer.Serialize(sw, UnfinishedJobs);

            return sw.ToString();
        }

        protected virtual IEnumerable<T> DeserializeShareJobs(string serialized, KeyValueStorage kvs)
        {
            if (serialized != null)
            {
                XmlSerializer reader = new XmlSerializer(typeof(List<T>));
                return (List<T>)reader.Deserialize(new StringReader(serialized));
            }
            return new List<T>();
        }

        public abstract NewsSharerWindow CreateSharerWindow(NewsArticle newsArticle, KeyValueStorage kvs);

        public IEnumerable<IShareJob> GetUnfinishedJobs(KeyValueStorage kvs)
        {
            string storedXml = kvs.GetString(UnfinishedJobsKey);
            List<T> rawJobs = DeserializeShareJobs(storedXml, kvs).ToList();

            UnfinishedJobs = new List<T>();
            AddJobs(rawJobs, kvs, false);

            return rawJobs.Cast<IShareJob>();
        }
    }
}
