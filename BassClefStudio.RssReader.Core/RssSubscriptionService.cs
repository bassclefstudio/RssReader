﻿using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BassClefStudio.RssReader.Core
{
    /// <summary>
    /// A service that can manage a collection of <see cref="RssSubscription"/>s and build a feed of <see cref="SyndicationItem"/> 'stories' from them.
    /// </summary>
    public class RssSubscriptionService
    {
        /// <summary>
        /// A collection of <see cref="RssSubscription"/>s telling the <see cref="RssSubscriptionService"/> where to find feed info.
        /// </summary>
        public ObservableCollection<RssSubscription> Subscriptions { get; }

        /// <summary>
        /// A collection of <see cref="RssArticle"/> items from all <see cref="Subscriptions"/>.
        /// </summary>
        public ObservableCollection<RssArticle> Feed { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="RssSubscriptionService"/> has been initialized.
        /// </summary>
        public bool Initialized { get; private set; }

        internal ISettingsService SettingsService { get; }
        internal IDispatcherService DispatcherService { get; }
        
        /// <summary>
        /// Creates a new <see cref="RssSubscriptionService"/> from the dependent services.
        /// </summary>
        /// <param name="dispatcherService">The app's <see cref="IDispatcherService"/>.</param>
        /// <param name="settingsService">The app's <see cref="ISettingsService"/>.</param>
        public RssSubscriptionService(IDispatcherService dispatcherService, ISettingsService settingsService)
        {
            DispatcherService = dispatcherService;
            SettingsService = settingsService;
            Subscriptions = new ObservableCollection<RssSubscription>();
            Feed = new ObservableCollection<RssArticle>();
            Subscriptions.CollectionChanged += SubscriptionsChanged;
        }

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

            SynchronousTask updateTask = new SynchronousTask(UpdateSubscriptionsAsync);
            updateTask.RunTask();
        }

        private void SubscriptionInfoChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SynchronousTask updateTask = new SynchronousTask(UpdateSubscriptionsAsync);
            updateTask.RunTask();
        }

        private async Task UpdateSubscriptionsAsync()
        {
            await SettingsService.SetValue("Subscriptions", Subscriptions.ToArray());
        }

        /// <summary>
        /// Initializes the <see cref="Subscriptions"/> from settings and calls <see cref="BuildFeedAsync"/>.
        /// </summary>
        public async Task InitializeAsync()
        {
            if (!Initialized)
            {
                Initialized = true;
                if (await SettingsService.ContainsKey("Subscriptions"))
                {
                    var subs = await SettingsService.GetValue<RssSubscription[]>("Subscriptions");
                    await DispatcherService.RunOnUIThreadAsync(() =>
                    {
                        Subscriptions.Clear();
                        Subscriptions.AddRange(subs);
                    });
                    await BuildFeedAsync();
                }
            }
        }

        /// <summary>
        /// Builds the <see cref="Feed"/> list from the current <see cref="Subscriptions"/>.
        /// </summary>
        public async Task BuildFeedAsync()
        {
            await DispatcherService.RunOnUIThreadAsync(() =>
            {
                Feed.Clear();
            });
            List<Task> tasks = new List<Task>();
            foreach (var sub in Subscriptions.ToArray())
            {
                tasks.Add(DispatcherService.RunOnUIThreadAsync(() => AddFeedInfo(sub)));
            }

            foreach (var t in tasks)
            {
                await t;
            }
        }

        private void AddFeedInfo(RssSubscription subscription)
        {
            Rss20FeedFormatter rssFormatter;
            using (var xmlReader = XmlReader.Create(subscription.Url))
            {
                rssFormatter = new Rss20FeedFormatter();
                rssFormatter.ReadFrom(xmlReader);
            }

            Feed.AddRange(rssFormatter.Feed.Items.Select(i => new RssArticle(subscription, i)));
        }
    }
}
