using Autofac;
using BassClefStudio.AppModel.Background;
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
            builder.RegisterAssemblyTypes(typeof(RssReaderApp).Assembly)
                .AssignableTo<IBackgroundTask>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyTypes(typeof(RssReaderApp).Assembly)
                .AssignableTo<IRssService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
