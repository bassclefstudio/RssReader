using BassClefStudio.AppModel.Navigation;
using BassClefStudio.RssReader.Core;
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

namespace BassClefStudio.RssReader.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavPage : Page, IView<NavViewModel>
    {
        /// <inheritdoc/>
        public NavViewModel ViewModel { get; set; }

        internal INavigationService NavService { get; }
        public NavPage(INavigationService navService)
        {
            NavService = navService;
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            if (NavService is UwpNavigationService uwpNavService)
            {
                uwpNavService.CurrentFrame = this.NavFrame;
            }
        }

        private void NavigationItemChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            ViewModel.Navigate(args.SelectedItem as NavigationItem);
        }
    }
}
