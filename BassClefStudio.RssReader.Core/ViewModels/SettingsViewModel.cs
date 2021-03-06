﻿using BassClefStudio.AppModel.Navigation;
using BassClefStudio.RssReader.Model;
using BassClefStudio.RssReader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.RssReader.ViewModels
{
    public class SettingsViewModel : IViewModel
    {
        public ObservableCollection<RssSubscription> Subscriptions { get; }

        public IRssService RssService { get; }
        public SettingsViewModel(IRssService rssService)
        {
            RssService = rssService;
            Subscriptions = RssService.Subscriptions;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await RssService.GetSubscriptionsAsync();
            await RssService.GetFeedAsync();
        }

        public void AddItem()
        {
            Subscriptions.Add(new RssSubscription());
        }

        public void RemoveItem(RssSubscription subscription)
        {
            Subscriptions.Remove(subscription);
        }
    }
}
