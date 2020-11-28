using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.RssReader.Core
{
    /// <summary>
    /// Represents a user-defined subscription to an RSS or Atom feed source.
    /// </summary>
    public class RssSubscription : Observable
    {
        private string name;
        /// <summary>
        /// The user-friendly name for this subscription.
        /// </summary>
        public string Name { get => name; set => Set(ref name, value); }

        private string url;
        /// <summary>
        /// The link to the RSS or Atom feed XML.
        /// </summary>
        public string Url { get => url; set => Set(ref url, value); }

        private bool notify;
        /// <summary>
        /// A <see cref="bool"/> indicating whether the app should check for updates to this <see cref="RssSubscription"/> in the background.
        /// </summary>
        public bool Notify { get => notify; set => Set(ref notify, value); }
    }
}
