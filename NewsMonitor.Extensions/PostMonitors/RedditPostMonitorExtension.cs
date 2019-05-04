using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.Data.Database;
using NewsMonitor.Data.Models;
using NewsMonitor.Extensions.NewsSharers.Reddit.Common;
using NewsMonitor.WPF.Extensions;
using NewsMonitor.WPF.Views;
using RedditSharp;
using RedditSharp.Things;

namespace NewsMonitor.Extensions.PostMonitors
{
    public class RedditPostMonitorExtension : IPostMonitorExtension
    {
        public event EventHandler<NeedsAttentionEventArgs> NeedsAttention;

        public string Domain => "reddit.com";
        
        public TimeSpan TimeSpan => TimeSpan.FromDays(1);

        public string ShareSettingsWith => "Reddit";

        public string Name => "Reddit monitor";

        public RedditPostMonitorExtension()
        {

        }


        public async Task Monitor(IEnumerable<ShareJobResult> results, KeyValueStorage kvs)
        {
            RedditSharpReader reader = new RedditSharpReader(new RedditSettings(kvs));

            foreach (ShareJobResult result in results)
            {
                Post post = await reader.GetPostInfo(result.Url);
                int upvoteThreshold = kvs.GetInteger(RedditPostMonitorSettingsPage.UpvoteThreshold, 5);
                int commentThreshold = kvs.GetInteger(RedditPostMonitorSettingsPage.CommentTheshold, 5);
                if (post.Upvotes >= upvoteThreshold && post.CommentCount >= commentThreshold)
                {
                    NeedsAttention?.Invoke(this, new NeedsAttentionEventArgs(result, 
                         $"{result.Url} needs attention. {post.Upvotes} upvotes, {post.CommentCount} comments"));
                }
                System.Diagnostics.Debug.WriteLine($"upvoteThreshold = {upvoteThreshold}, commentThreshold = {commentThreshold}");
                System.Diagnostics.Debug.WriteLine($"post.Upvotes = {post.Upvotes}, post.CommentCount = {post.CommentCount}");
            }
        }

        public SettingsPage CreateSettingsPage()
        {
            return new RedditPostMonitorSettingsPage();
        }
    }
}
