using BassClefStudio.AppModel.Background;
using BassClefStudio.AppModel.Notifications;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.RssReader.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BassClefStudio.RssReader.Background
{
    /// <summary>
    /// A test <see cref="IBackgroundTask"/> that writes dates to a file.
    /// </summary>
    public class NotificationBackgroundTask : IBackgroundTask
    {
        /// <inheritdoc/>
        public BackgroundTaskCapability Capabilities { get; } = BackgroundTaskCapability.Internet;

        /// <inheritdoc/>
        public BackgroundTaskTrigger Trigger { get; } = new TimeBackgroundTaskTrigger(new TimeSpan(1, 0, 0), true);

        /// <inheritdoc/>
        public bool Enabled { get; } = true;

        /// <inheritdoc/>
        public string Id { get; } = "Notification";

        internal INotificationService NotificationService { get; }
        internal IRssService RssService { get; }
        public NotificationBackgroundTask(IRssService rssService, INotificationService notificationService)
        {
            RssService = rssService;
            NotificationService = notificationService;
        }

        /// <inheritdoc/>
        public async Task RunAsync()
        {
            await RssService.InitializeAsync();
            await RssService.SyncFeedAsync();
            var unread = RssService.Feed.Where(f => !f.Read).ToArray();
            foreach (var item in unread)
            {
                Debug.WriteLine($"{item.Subscription.Name}:{item.Title}");
                await NotificationService.ShowNotificationAsync(new NotificationContent($"New post from {item.Subscription.Name}", item.Title));
                item.Read = true;
            }
        }
    }
}
