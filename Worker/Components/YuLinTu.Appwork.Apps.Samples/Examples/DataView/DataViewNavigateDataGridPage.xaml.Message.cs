using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Samples.SampleData;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.DataView
{
    public partial class DataViewNavigateDataGridPageViewModel :
        YuLinTu.Messages.Workspace.IMessageHandlerInstallAccountData,
        YuLinTu.Messages.Workspace.IMessageHandlerUninstallAccountData,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerInitializeWorkpageContentCompleted,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigateItem,
        YuLinTu.Messages.Workpage.IMessageHandlerNavigateTo
    {
        #region Methods

        #region Methods - Message

        public void InstallWorkpageContent(object sender, InstallWorkpageContentEventArgs e)
        {
        }

        public void UninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
        }

        public void InitializeWorkpageContentCompleted(object sender, InitializeWorkpageContentCompletedEventArgs e)
        {
        }

        public void InstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void UninstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void InstallNavigateItem(object sender, InstallNavigateItemMsgEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var navigator = Workpage.Properties.TryGetValue<Navigator>("Navigator", null);
                if (navigator == null || !navigator.RootItemAutoExpand)
                    return;

                navigator.RootItemAutoExpand = false;

                var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Appwork.Apps.Samples;component/Examples/DataView/NavigateDataGridRes.xaml") };
                var key = new DataTemplateKey(typeof(NavigateItemCountry));
                navigator.RegisterItemTemplate(typeof(NavigateItemCountry), dic[key] as DataTemplate);
            }));

            if (e.Instance.Root == null || e.Instance.Root.Object == null)
                return;

            if (e.Instance.Root.Object.ToString() == "ROOT")
            {
                GetAllCountries().ForEach(c => e.Instance.Items.Add(CreateCountryItem(c)));
                return;
            }

            var root = e.Instance.Root.Object as CountryItem;
            if (root == null)
                return;

            GetCities(root).ForEach(c => e.Instance.Items.Add(CreateCityItem(c)));
        }

        public void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            NavigateObject = e.Object == null ? null : e.Object.Object;
        }

        #endregion

        #region Methods - Private

        private List<CountryItem> GetAllCountries()
        {
            return SampleDataProvider.GetOrders().
                Select(c => c.ShipCountry).
                Distinct().
                Select(c => new CountryItem() { Name = c }).
                ToList();
        }

        private List<CityItem> GetCities(CountryItem country)
        {
            return SampleDataProvider.GetOrders().
                Where(c => c.ShipCountry == country.Name).
                Select(c => c.ShipCity).
                Distinct().
                Select(c => new CityItem() { Name = c }).
                ToList();
        }

        private NavigateItem CreateCountryItem(CountryItem c)
        {
            var item = new NavigateItemCountry();
            item.CanOpen = true;
            item.Name = c.Name;
            item.Object = c;

            return item;
        }

        private NavigateItem CreateCityItem(CityItem c)
        {
            var item = new NavigateItem();
            item.CanOpen = false;
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/24/document.png"));
            item.Name = c.Name;
            item.Object = c;


            return item;
        }

        #endregion

        #endregion
    }
}
