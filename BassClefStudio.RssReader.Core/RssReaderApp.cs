using Autofac;
using BassClefStudio.AppModel.Background;
using BassClefStudio.AppModel.Helpers;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Serialization;
using BassClefStudio.RssReader.Model;
using BassClefStudio.RssReader.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.RssReader
{
    public class RssReaderApp : App
    {
        public RssReaderApp() : base("BassClefStudio.Rss", typeof(RssReaderApp).Assembly.GetName().Version)
        { }

        /// <inheritdoc/>
        protected override void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterViewModels(typeof(RssReaderApp).Assembly);
            builder.RegisterAssemblyTypes(typeof(RssReaderApp).Assembly)
                .AssignableTo<IBackgroundTask>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyTypes(typeof(RssReaderApp).Assembly)
                .AssignableTo<IRssService>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterInstance<ISerializationService>(
                new SerializationService(typeof(RssReaderApp).Assembly));
            builder.RegisterSettingsContext<RssSubscription[]>("Subscriptions");
            builder.RegisterSettingsContext<RssArticle[]>("feed.json", "Feed");
        }
    }
}
