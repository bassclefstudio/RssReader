using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using BassClefStudio.RssReader.ViewModels;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BassClefStudio.RssReader.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, IView<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }

        internal IDispatcherService DispatcherService { get; }
        public MainPage(IDispatcherService dispatcherService)
        {
            DispatcherService = dispatcherService;
            InitializeComponent();
        }

        public void Initialize()
        {
            this.DataContext = ViewModel;
        }

        private void WebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            SynchronousTask styleTask = new SynchronousTask(
                    () => DispatcherService.RunOnUIThreadAsync(
                        () => SetWebViewStyle(sender as WebView2)));
            styleTask.RunTask();
        }

        private string GetTextColor() => "white";
        private string SetStyleString() => $@"function SetStyle() {{ document.body.style.backgroundColor='#444444'; document.body.style.color = '{GetTextColor()}'; document.body.style.fontFamily = 'Segoe UI, sans-serif'; }} SetStyle();";
        private async Task SetWebViewStyle(WebView2 webView)
        {
            await webView.ExecuteScriptAsync(SetStyleString());
            Debug.WriteLine("Loaded");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SynchronousTask refreshTask = new SynchronousTask(ViewModel.RefreshAsync);
            refreshTask.RunTask();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenSettings();
        }
    }
}
