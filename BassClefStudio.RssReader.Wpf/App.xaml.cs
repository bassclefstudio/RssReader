using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BassClefStudio.RssReader.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication
    {
        public App() : base(new RssReaderApp(), typeof(App).Assembly)
        { }
    }
}
