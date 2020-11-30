using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using BassClefStudio.RssReader.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BassClefStudio.RssReader.Services
{
    /// <summary>
    /// Represents a service that can manage RSS/Atom feeds.
    /// </summary>
    public interface IRssService : INotifyPropertyChanged
    {
        /// <summary>
        /// The current RSS/Atom subscriptions, as <see cref="RssSubscription"/>s, which are used to sync feed data.
        /// </summary>
        ObservableCollection<RssSubscription> Subscriptions { get; }

        /// <summary>
        /// The current collection of <see cref="RssArticle"/>s from the given <see cref="Subscriptions"/>.
        /// </summary>
        ObservableCollection<RssArticle> Feed { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether content is being loaded.
        /// </summary>
        bool Loading { get; }

        /// <summary>
        /// Retrieves the locally cached <see cref="RssSubscription"/>s from local storage.
        /// </summary>
        Task GetSubscriptionsAsync();

        /// <summary>
        /// Retrieves the locally cached <see cref="RssArticle"/>s from local storage.
        /// </summary>
        Task GetFeedAsync();

        /// <summary>
        /// Saves the locally cached <see cref="RssSubscription"/>s to local storage.
        /// </summary>
        Task SaveSubscriptionsAsync();

        /// <summary>
        /// Saves the locally cached <see cref="RssArticle"/>s to local storage.
        /// </summary>
        Task SaveFeedAsync();

        /// <summary>
        /// Syncs the <see cref="Feed"/> collection with <see cref="RssArticle"/> information collected from the online <see cref="RssSubscription"/> services.
        /// </summary>
        Task SyncFeedAsync();
    }

    internal class RssService : Observable, IRssService
    {
        /// <inheritdoc/>
        public ObservableCollection<RssSubscription> Subscriptions { get; }

        /// <inheritdoc/>
        public ObservableCollection<RssArticle> Feed { get; }

        private bool loading;
        /// <inheritdoc/>
        public bool Loading { get => loading; private set => Set(ref loading, value); }

        internal IStorageService StorageService { get; }
        internal ISettingsService SettingsService { get; }
        internal IDispatcherService DispatcherService { get; }
        /// <summary>
        /// Creates a new <see cref="RssService"/> from the required dependencies.
        /// </summary>
        public RssService(IStorageService storageService, ISettingsService settingsService, IDispatcherService dispatcherService)
        {
            Subscriptions = new ObservableCollection<RssSubscription>();
            Feed = new ObservableCollection<RssArticle>();

            StorageService = storageService;
            SettingsService = settingsService;
            DispatcherService = dispatcherService;

            Subscriptions.CollectionChanged += SubscriptionsChanged;
            Feed.CollectionChanged += FeedChanged;
        }

        #region Changes

        private void SubscriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var i in e.NewItems.OfType<RssSubscription>())
                {
                    i.PropertyChanged += SubscriptionInfoChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var i in e.OldItems.OfType<RssSubscription>())
                {
                    i.PropertyChanged -= SubscriptionInfoChanged;
                }
            }

            SynchronousTask updateTask = new SynchronousTask(SaveSubscriptionsAsync);
            updateTask.RunTask();
        }

        private void FeedChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var i in e.NewItems.OfType<RssArticle>())
                {
                    i.PropertyChanged += FeedItemChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var i in e.OldItems.OfType<RssArticle>())
                {
                    i.PropertyChanged -= FeedItemChanged;
                }
            }
        }

        private void SubscriptionInfoChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SynchronousTask updateTask = new SynchronousTask(SaveSubscriptionsAsync);
            updateTask.RunTask();
        }

        private void FeedItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SynchronousTask updateTask = new SynchronousTask(SaveFeedAsync);
            updateTask.RunTask();
        }

        #endregion
        #region Interface

        /// <inheritdoc/>
        public async Task GetSubscriptionsAsync()
        {
            if (await SettingsService.ContainsKey("Subscriptions"))
            {
                await DispatcherService.RunOnUIThreadAsync(() => Loading = true);
                var subs = await SettingsService.GetValue<RssSubscription[]>("Subscriptions");
                await DispatcherService.RunOnUIThreadAsync(() =>
                {
                    Subscriptions.Clear();
                    if (subs != null)
                    {
                        Subscriptions.AddRange(subs);
                    }
                    Loading = false;
                });
            }
        }

        /// <inheritdoc/>
        public async Task SaveSubscriptionsAsync()
        {
            await DispatcherService.RunOnUIThreadAsync(() => Loading = true);
            await SettingsService.SetValue("Subscriptions", Subscriptions.ToArray());
            await DispatcherService.RunOnUIThreadAsync(() => Loading = false);
        }

        /// <inheritdoc/>
        public async Task GetFeedAsync()
        {
            await DispatcherService.RunOnUIThreadAsync(() => Loading = true);
            var feedFile = await StorageService.AppDataFolder.CreateFileAsync("Feed.json", CollisionOptions.OpenExisting);
            string json = await feedFile.ReadTextAsync();
            var articles = JsonConvert.DeserializeObject<RssArticle[]>(json);
            await DispatcherService.RunOnUIThreadAsync(() =>
            {
                Feed.Clear();
                if (articles != null)
                {
                    foreach (var a in articles)
                    {
                        a.Subscription = Subscriptions.FirstOrDefault(s => s.Name == a.SubscriptionName);
                        Feed.Add(a);
                    }
                }
                Loading = false;
            });
        }

        /// <inheritdoc/>
        public async Task SaveFeedAsync()
        {
            await DispatcherService.RunOnUIThreadAsync(() => Loading = true);
            var feedFile = await StorageService.AppDataFolder.CreateFileAsync("Feed.json", CollisionOptions.OpenExisting);
            await feedFile.WriteTextAsync(JsonConvert.SerializeObject(Feed.ToArray()));
            await DispatcherService.RunOnUIThreadAsync(() => Loading = false);
        }

        /// <inheritdoc/>
        public async Task SyncFeedAsync()
        {
            await DispatcherService.RunOnUIThreadAsync(() => Loading = true);
            List<Task<IEnumerable<RssArticle>>> tasks = new List<Task<IEnumerable<RssArticle>>>();
            foreach (var sub in Subscriptions)
            {
                tasks.Add(GetArticlesAsync(sub));
            }

            List<RssArticle> articles = new List<RssArticle>();
            foreach (var task in tasks)
            {
                articles.AddRange(await task);
            }

            await DispatcherService.RunOnUIThreadAsync(() =>
            {
                Feed.Sync(articles, (a1, a2) => a1.Id == a2.Id, false);
                Feed.RemoveRange(Feed.Where(a => !Subscriptions.Contains(a.Subscription)).ToArray());
                Loading = false;
            });
            await SaveFeedAsync();
        }

        private static SyndicationFeedFormatter[] Formatters { get; }
            = new SyndicationFeedFormatter[]
            {
                new Rss20FeedFormatter(),
                new Atom10FeedFormatter()
            };

        private async Task<IEnumerable<RssArticle>> GetArticlesAsync(RssSubscription subscription)
        {
            return await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(subscription?.Url))
                {
                    try
                    {
                        using (var xmlReader = XmlReader.Create(subscription.Url))
                        {
                            var formatter = Formatters.FirstOrDefault(f => f.CanRead(xmlReader));
                            if (formatter == null)
                            {
                                Debug.WriteLine($"No format found for given XML: {subscription.Name}");
                                return new RssArticle[0];
                            }
                            else
                            {
                                formatter.ReadFrom(xmlReader);
                                return formatter.Feed.Items.Select(i => new RssArticle(subscription, i));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Loading feed for {subscription.Name} failed: {ex}");
                        return new RssArticle[0];
                    }
                }
                else
                {
                    Debug.WriteLine($"Feed does not exist: {subscription?.Name}");
                    return new RssArticle[0];
                }
            });
        }

        #endregion
    }
}
