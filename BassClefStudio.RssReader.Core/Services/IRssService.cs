using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using BassClefStudio.NET.Sync;
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
        /// A <see cref="bool"/> indicating an ongoing operation is loading in the background.
        /// </summary>
        bool Loading { get; }

        /// <summary>
        /// Syncs the <see cref="Feed"/> collection with <see cref="RssArticle"/> information collected from the online <see cref="RssSubscription"/> services.
        /// </summary>
        Task SyncFeedAsync();

        /// <summary>
        /// Initializes the <see cref="Feed"/> and <see cref="Subscriptions"/> asynchronously.
        /// </summary>
        Task InitializeAsync();
    }

    internal class RssService : Observable, IRssService
    {
        /// <inheritdoc/>
        public ObservableCollection<RssSubscription> Subscriptions { get; }

        /// <inheritdoc/>
        public ObservableCollection<RssArticle> Feed { get; }

        private bool loading;
        /// <inheritdoc/>
        public bool Loading { get => loading; set => Set(ref loading, value); }

        internal ISyncItem<RssSubscription[]> SubLink { get; }
        internal ISyncItem<RssArticle[]> FeedLink { get; }
        internal IDispatcherService DispatcherService { get; }
        /// <summary>
        /// Creates a new <see cref="RssService"/> from the required dependencies.
        /// </summary>
        public RssService(ISyncItem<RssSubscription[]> subscriptions, ISyncItem<RssArticle[]> feed, IDispatcherService dispatcherService)
        {
            Subscriptions = new ObservableCollection<RssSubscription>();
            Feed = new ObservableCollection<RssArticle>();
            SubLink = subscriptions;
            FeedLink = feed;
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

            SynchronousTask updateTask = new SynchronousTask(PushSubsAsync);
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

        private void SubscriptionInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronousTask updateTask = new SynchronousTask(PushSubsAsync);
            updateTask.RunTask();
        }

        private void FeedItemChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronousTask updateTask = new SynchronousTask(PushFeedAsync);
            updateTask.RunTask();
        }

        #endregion
        #region Interface
        
        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            if (!Subscriptions.Any())
            {
                await SubLink.UpdateAsync();
                Subscriptions.AddRange(SubLink.Item);
                await FeedLink.UpdateAsync();
                Feed.AddRange(FeedLink.Item);
            }
        }

        public async Task PushSubsAsync()
        {
            SubLink.Item = Subscriptions.ToArray();
            await SubLink.PushAsync();
        }

        public async Task PushFeedAsync()
        {
            FeedLink.Item = Feed.ToArray();
            await FeedLink.PushAsync();
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
            await PushFeedAsync();
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
