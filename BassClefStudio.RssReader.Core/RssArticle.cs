using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;

namespace BassClefStudio.RssReader.Core
{
    /// <summary>
    /// Represents a <see cref="SyndicationItem"/> collected from a specific <see cref="RssSubscription"/>.
    /// </summary>
    public class RssArticle
    {
        public RssSubscription Subscription { get; }
        public SyndicationItem Content { get; }

        public RssArticle(RssSubscription subscription, SyndicationItem item)
        {
            Subscription = subscription;
            Content = item;
        }
    }
}
