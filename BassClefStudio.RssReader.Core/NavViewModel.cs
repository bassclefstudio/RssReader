using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.RssReader.Core
{
    public class NavViewModel : Observable, IShellHandler
    {
        public bool Enabled { get; } = true;

        public ObservableCollection<NavigationItem> NavItems { get; }

        private NavigationItem selectedItem;
        public NavigationItem SelectedItem { get => selectedItem; set => Set(ref selectedItem, value); }

        internal App App { get; }
        internal IDispatcherService DispatcherService { get; }
        public NavViewModel(App app, IDispatcherService dispatcherService)
        {
            App = app;
            DispatcherService = dispatcherService;
            NavItems = new ObservableCollection<NavigationItem>();
            NavItems.Add(new NavigationItem("Main", '\uE10F', typeof(MainViewModel)));
            NavItems.Add(new NavigationItem("Settings", '\uE713', typeof(SettingsViewModel)));

            App.Navigated += Navigated;
        }

        private void Navigated(object sender, NavigatedEventArgs e)
        {
            SelectedItem = NavItems.FirstOrDefault(i => i.ViewModelType == e.NavigatedViewModel.GetType());
        }

        public void Navigate(NavigationItem item)
        {
            if (item != null && item != SelectedItem)
            {
                App.NavigateReflection(item.ViewModelType);
            }
        }

        public async Task InitializeAsync()
        { }
    }

    public class NavigationItem
    {
        /// <summary>
        /// The <see cref="string"/> name of the <see cref="NavigationItem"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A <see cref="char"/> icon for the <see cref="NavigationItem"/>.
        /// </summary>
        public char Icon { get; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IViewModel"/> to navigate to.
        /// </summary>
        public Type ViewModelType { get; }

        public NavigationItem(string name, char icon, Type viewModelType)
        {
            Name = name;
            Icon = icon;
            ViewModelType = viewModelType;
        }
    }
}
