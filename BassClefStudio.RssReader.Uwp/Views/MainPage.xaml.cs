using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using BassClefStudio.RssReader.Model;
using BassClefStudio.RssReader.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BassClefStudio.RssReader.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IView<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }

        public AdvancedCollectionView FeedView { get; }

        public IDispatcherService DispatcherService { get; set; }
        public MainPage(IDispatcherService dispatcherService)
        {
            FeedView = new AdvancedCollectionView();
            DispatcherService = dispatcherService;
            this.InitializeComponent();
        }

        public void Initialize()
        {
            FeedView.SortDescriptions.Add(new SortDescription("PostedDate", SortDirection.Descending));
            FeedView.Source = ViewModel.Feed;
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            SynchronousTask refreshTask = new SynchronousTask(ViewModel.RefreshAsync);
            refreshTask.RunTask();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenSettings();
        }

        private void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            SynchronousTask styleTask = new SynchronousTask(
                () => DispatcherService.RunOnUIThreadAsync(
                    () => SetWebViewStyle(sender)));
            styleTask.RunTask();
        }

        private string GetTextColor() => (App.Current.RequestedTheme == ApplicationTheme.Light) ? "black" : "white";
        private string[] SetStyleString() => new string[] { $@"function SetStyle() {{ document.body.style.color = '{GetTextColor()}'; document.body.style.fontFamily = 'Segoe UI, sans-serif'; }} SetStyle();" };
        private async Task SetWebViewStyle(WebView webView)
        {
            await webView.InvokeScriptAsync("eval", SetStyleString());
            Debug.WriteLine("Loaded");
        }

        private void FeedSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as MasterDetailsView).SelectedItem is RssArticle article)
            {
                article.Read = true;
            }
        }
    }
}
