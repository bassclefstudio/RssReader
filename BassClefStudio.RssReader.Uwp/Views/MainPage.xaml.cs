using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using BassClefStudio.RssReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BassClefStudio.RssReader.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IView<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }

        public IDispatcherService DispatcherService { get; set; }
        public MainPage(IDispatcherService dispatcherService)
        {
            DispatcherService = dispatcherService;
            this.InitializeComponent();
        }

        public void Initialize()
        { }

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
    }
}
