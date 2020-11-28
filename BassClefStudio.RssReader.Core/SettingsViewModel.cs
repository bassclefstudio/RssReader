using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.RssReader.Core
{
    public class SettingsViewModel : IViewModel
    {
        public ObservableCollection<RssSubscription> Subscriptions { get; }

        internal RssSubscriptionService RssService { get; }
        public SettingsViewModel(RssSubscriptionService rssService)
        {
            RssService = rssService;
            Subscriptions = RssService.Subscriptions;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await RssService.InitializeAsync();
        }

        public void AddItem()
        {
            Subscriptions.Add(new RssSubscription());
        }
    }
}
