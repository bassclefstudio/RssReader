using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.RssReader.Core
{
    public class RssReaderApp : App
    {
        /// <inheritdoc/>
        protected override void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterViewModels(typeof(RssReaderApp).Assembly);
            builder.RegisterType<RssSubscriptionService>()
                .SingleInstance()
                .AsSelf();
        }
    }
}
