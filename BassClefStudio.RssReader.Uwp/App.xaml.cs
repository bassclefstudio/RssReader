using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.RssReader.Core;

namespace BassClefStudio.RssReader.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : UwpApplication
    {
        public App() : base(new RssReaderApp(), typeof(App).Assembly)
        { }
    }
}
