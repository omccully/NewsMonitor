using NewsMonitor.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewsMonitor.WPF.Settings;
using NewsMonitor.WPF.Settings.Mappings;
using NewsMonitor.WPF.Views.EditableTreeView;

namespace NewsMonitor.Extensions.NewsSharers.Reddit
{
    /// <summary>
    /// Interaction logic for RedditNewsSharerSettingsPage.xaml
    /// </summary>
    public partial class RedditNewsSharerSettingsPage : SettingsPage
    {
        IEnumerable<EditableTreeViewLevelRule> EditableTreeViewRules = new List<EditableTreeViewLevelRule>()
        {
            new EditableTreeViewLevelRule("<section>", true),
            new EditableTreeViewLevelRule("<subreddit>", true)
        };

        public RedditNewsSharerSettingsPage()
        {
            InitializeComponent();

            SettingsMappings.Add(
                new TextBoxSettingsMapping(RedditUsernameKey, null, RedditUsernameTextBox));
            SettingsMappings.Add(
                new PasswordBoxSettingsMapping(RedditPasswordKey, null, RedditPasswordTextBox));
            SettingsMappings.Add(
                new TextBoxSettingsMapping(RedditClientIdKey, null, RedditClientIdTextBox));
            SettingsMappings.Add(
                new PasswordBoxSettingsMapping(RedditClientSecretKey, null, RedditClientSecretTextBox));
            SettingsMappings.Add(
                new EditableTreeViewSettingsMapping(RedditDefaultSubredditsKey, null, SubredditOptionsTreeView, EditableTreeViewRules));
        }

        public const string RedditUsernameKey = "RedditUsername";
        public const string RedditPasswordKey = "RedditPassword";
        public const string RedditClientIdKey = "RedditClientId";
        public const string RedditClientSecretKey = "RedditClientSecret";
        public const string RedditDefaultSubredditsKey = "DefaultSubreddits";

        const string SectionText = "<section>";
        const string SubredditText = "<subreddit>";

        public override void Restore()
        {
            base.Restore();



            /*SubredditOptionsTreeView.KeyUp += SubredditOptionsTreeView_KeyUp;
            foreach(object item in SubredditOptionsTreeView.Items)
            {
                ((TreeViewItem)item).IsExpanded = true;
            }
            AddAddTextBox(SubredditOptionsTreeView, SectionText, AddTextBoxFlag.CanGoDeeper | AddTextBoxFlag.Unique);
            foreach(object child in SubredditOptionsTreeView.Items)
            {
                TreeViewItem tvi = child as TreeViewItem;
                if (tvi == null) continue;
                AddAddTextBox(tvi, SubredditText);
            }*/
        }

       /* private void SubredditOptionsTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("SubredditOptionsTreeView_KeyUp " + e.Key);
            if (e.Key != Key.Delete) return;

            e.Handled = true;

            DeleteFirstSelectedTreeViewItem(SubredditOptionsTreeView, true);
        }

        bool DeleteFirstSelectedTreeViewItem(ItemsControl parent, bool recursive = false)
        {
            List<object> itemsCopy = new List<object>(parent.Items.Cast<object>());
            foreach (object item in itemsCopy)
            {
                TreeViewItem tvi = item as TreeViewItem;
                if (tvi == null) continue;

                if (recursive)
                {
                    if(DeleteFirstSelectedTreeViewItem(tvi, true))
                    {
                        return true;
                    }
                }

                if (tvi.IsSelected)
                {
                    Console.WriteLine("delete " + tvi.Header);
                    parent.Items.Remove(item);
                    return true;
                }
            }
            return false;
        }


        enum AddTextBoxFlag
        {
            None = 0x0,
            CanGoDeeper = 0x1,
            Unique = 0x2
        }

        class AddTextBoxRules
        {
            public string HelpText { get; private set; }
            //public bool CanGoDeeper { get; private set; }
            public bool Unique { get; private set; }

            public AddTextBoxRules(string helpText, bool unique = false)
            {
                this.HelpText = helpText;
               // this.CanGoDeeper = canGoDeeper;
                this.Unique = unique;

            }
        }

        TextBox AddAddTextBox(ItemsControl control, string helpText, AddTextBoxFlag flags = AddTextBoxFlag.None)
        {
            TextBox tb = new TextBox()
            {
                MinWidth = 50,
                Text = helpText
            };

            tb.GotKeyboardFocus += (o, e) =>
            {
                if (tb.Text == helpText) tb.Text = "";
            };

            tb.LostKeyboardFocus += (o, e) =>
            {
                if(String.IsNullOrWhiteSpace(tb.Text)) tb.Text = helpText;
            };

            tb.KeyUp += (o, e) =>
            {
                tb.Background = new SolidColorBrush(Colors.White);
                if (e.Key != Key.Enter) return;

                e.Handled = true;

                // enter button pressed

                string newItemText = tb.Text;

                if((flags & AddTextBoxFlag.Unique) == AddTextBoxFlag.Unique)
                {
                    if(ItemsControlHasChildWithHeader(control, newItemText))
                    {
                        tb.Background = new SolidColorBrush(Colors.Red);
                        return;
                    }
                }
                    

                TreeViewItem tvi = new TreeViewItem()
                {
                    Header = newItemText
                };
                if ((flags & AddTextBoxFlag.CanGoDeeper) == AddTextBoxFlag.CanGoDeeper)
                {
                    tvi.IsExpanded = true;
                    TextBox innerTb = AddAddTextBox(tvi, helpText,
                        flags & AddTextBoxFlag.Unique);
                    tb.Text = "";
                    Keyboard.Focus(innerTb);
                }
                else
                {
                    tb.Text = "";
                }
                control.Items.Insert(control.Items.Count - 1, tvi);

                //Keyboard.ClearFocus();
            };

            control.Items.Add(tb);
            return tb;
        }

        bool ItemsControlHasChildWithHeader(ItemsControl control, string testStr)
        {
            foreach (object item in control.Items)
            {
                TreeViewItem foo = item as TreeViewItem;

                if (foo != null && foo.Header.ToString() == testStr)
                {
                    return true;
                }
            }

            return false;
        }*/
    }
}
