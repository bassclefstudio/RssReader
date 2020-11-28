using BassClefStudio.AppModel.Navigation;
using BassClefStudio.RssReader.Model;
using BassClefStudio.RssReader.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BassClefStudio.RssReader.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsDialog : ContentDialog, IView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsDialog()
        {
            this.InitializeComponent();
        }

        public void Initialize()
        { }

        private void AddItem(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            ViewModel.AddItem();
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            if((sender as FrameworkElement)?.DataContext is RssSubscription sub)
            {
                ViewModel.RemoveItem(sub);
            }
        }
    }
}
