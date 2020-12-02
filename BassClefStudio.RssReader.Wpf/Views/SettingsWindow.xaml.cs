using BassClefStudio.AppModel.Navigation;
using BassClefStudio.RssReader.Model;
using BassClefStudio.RssReader.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BassClefStudio.RssReader.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, IView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            this.DataContext = ViewModel;
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveItem((sender as Button)?.DataContext as RssSubscription);
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            ViewModel.AddItem();
        }
    }
}
