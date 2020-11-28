using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
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

namespace BassClefStudio.RssReader.Core
{
    public class MainViewModel : Observable, IViewModel, IActivationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; } = true;

        public ObservableCollection<RssArticle> Feed { get; }

        internal RssSubscriptionService RssService { get; }
        public MainViewModel(RssSubscriptionService rssService)
        {
            RssService = rssService;
            Feed = RssService.Feed;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await RssService.InitializeAsync();
        }

        /// <inheritdoc/>
        public bool CanHandle(IActivatedEventArgs args)
        {
            return args is LaunchActivatedEventArgs;
        }

        /// <inheritdoc/>
        public void Activate(IActivatedEventArgs args)
        { }
    }
}
