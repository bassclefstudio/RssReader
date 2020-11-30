using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using BassClefStudio.RssReader.Model;
using BassClefStudio.RssReader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.RssReader.ViewModels
{
    public class MainViewModel : Observable, IViewModel, IActivationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; } = true;

        public ObservableCollection<RssArticle> Feed { get; }

        public IRssService RssService { get; }
        internal App App { get; }
        public MainViewModel(App app, IRssService rssService)
        {
            App = app;
            RssService = rssService;
            Feed = RssService.Feed;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await RssService.GetSubscriptionsAsync();
            await RssService.GetFeedAsync();
        }

        /// <inheritdoc/>
        public bool CanHandle(IActivatedEventArgs args)
        {
            return args is LaunchActivatedEventArgs;
        }

        /// <inheritdoc/>
        public void Activate(IActivatedEventArgs args)
        { }

        public async Task RefreshAsync()
        {
            await RssService.SyncFeedAsync();
        }

        public void OpenSettings()
        {
            App.Navigate<SettingsViewModel>();
        }
    }
}
