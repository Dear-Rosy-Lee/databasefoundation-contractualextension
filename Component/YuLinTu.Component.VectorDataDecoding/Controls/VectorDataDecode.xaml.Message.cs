using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Appwork;
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
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = "责任说明",
                Message = " 1.‌测绘资质和资格‌：使用该软件应该具备从事测绘活动必须相应的测绘资质和资格。了解测绘活动应当遵守的国家规定的的要求，执行国家规定的测绘技术规范和标准。‌\r\n\r\n‌2.保密要求‌：使用该软件处理地理信息相关数据，必须严格遵守保密规定。任何单位和个人处理地理信息数据时，应当遵守法律法规和相关保密规定，不得危害国家安全，不得泄露国家秘密‌。\r\n\r\n3.‌数据管理和使用‌：涉密测绘成果的使用和管理必须严格遵守相关规定。本软件处理数据范围不超过25平方千米‌,且未经批准成果不得对外提供。",
                MessageGrade = eMessageGrade.Warn,
                CancelButtonVisibility = Visibility.Collapsed,
                ConfirmButtonVisibility = Visibility.Visible,
            }, (wb, we) =>
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
