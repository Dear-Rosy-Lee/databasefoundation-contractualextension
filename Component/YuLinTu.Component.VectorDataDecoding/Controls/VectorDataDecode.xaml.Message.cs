using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Component.VectorDataDecoding.Controls;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding
{
    public partial class VectorDataDecodeViewModel :
        YuLinTu.Messages.Workspace.IMessageHandlerInstallAccountData,
        YuLinTu.Messages.Workspace.IMessageHandlerUninstallAccountData,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerInitializeWorkpageContentCompleted
    //YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigateItem,
    //YuLinTu.Messages.Workpage.IMessageHandlerNavigateTo
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
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Components", "YuLinTu.Component.VectorDataDecoding", "Config.xml");
            var cmp = Serializer.DeserializeFromXmlFile<WinComponent>(fileName);
            if (cmp.Name.Contains(Constants.clientName))
            {
                Constants.ClientType = ClientEum.UploaDeclassifyDataClient;
                return;
            }
            var rsbd = new ResponsibleDialog();
            Workpage.Page.ShowDialog(rsbd, (wb, we) =>
            {

            });
        }

        public void InstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void UninstallAccountData(object sender, AccountEventArgs e)
        {
        }

        //public void NavigateTo(object sender, NavigateToMsgEventArgs e)
        //{
        //    NavigateObject = e.Object == null ? null : e.Object.Object;
        //}

        //public void InstallNavigateItem(object sender, InstallNavigateItemMsgEventArgs e)
        //{

        //    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        //    {
        //        var navigator = Workpage.Properties.TryGetValue<Navigator>("Navigator", null);
        //        if (navigator == null || !navigator.RootItemAutoExpand)
        //            return;

        //        navigator.RootItemAutoExpand = false;

        //        var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Appwork.Apps.Samples;component/Examples/DataView/NavigateDataGridRes.xaml") };
        //        var key = new DataTemplateKey(typeof(NavigateItemCountry));
        //        navigator.RegisterItemTemplate(typeof(NavigateItemCountry), dic[key] as DataTemplate);
        //    }));

        //    if (e.Instance.Root == null || e.Instance.Root.Object == null)
        //        return;

        //    if (e.Instance.Root.Object.ToString() == "ROOT")
        //    {
        //        GetAllCountries().ForEach(c => e.Instance.Items.Add(CreateCountryItem(c)));
        //        return;
        //    }

        //    var root = e.Instance.Root.Object as CountryItem;
        //    if (root == null)
        //        return;

        //    GetCities(root).ForEach(c => e.Instance.Items.Add(CreateCityItem(c)));
        //}

        #endregion

        #endregion
    }
}
