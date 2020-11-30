using BassClefStudio.NET.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;

namespace BassClefStudio.RssReader.Model
{
    /// <summary>
    /// Represents a <see cref="SyndicationItem"/> collected from a specific <see cref="RssSubscription"/>.
    /// </summary>
    public class RssArticle : Observable, IIdentifiable<string>
    {
        private string subscriptionName;
        public string SubscriptionName { get=> Subscription != null ? Subscription.Name : subscriptionName; set => Set(ref subscriptionName, value); }

        [JsonIgnore]
        public RssSubscription Subscription { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTimeOffset PostedDate { get; set; }

        public string[] Links { get; set; }

        private bool read = false;
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="RssArticle"/> was read.
        /// </summary>
        public bool Read { get => read; set => Set(ref read, value); }

        [JsonConstructor]
        internal RssArticle() { }

        /// <summary>
        /// Creates a new <see cref="RssArticle"/>.
        /// </summary>
        /// <param name="subscription">The attached <see cref="RssSubscription"/>.</param>
        /// <param name="item">The retrieved <see cref="SyndicationItem"/> from the web feed.</param>
        public RssArticle(RssSubscription subscription, SyndicationItem item)
        {
            Subscription = subscription;
            Id = $"{Subscription.Name}_{item.Id}";
            Body = item.Summary?.Text;
            Title = item.Title.Text;
            PostedDate = item.PublishDate.ToLocalTime();
            Links = item.Links.Select(l => l.Uri.ToString()).ToArray();
        }
    }
}
