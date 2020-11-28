using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.RssReader.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.RssReader
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
