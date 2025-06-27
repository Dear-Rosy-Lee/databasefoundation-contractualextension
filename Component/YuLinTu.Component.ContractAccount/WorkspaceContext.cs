/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账插件工作空间上下文
    /// </summary>
    public class WorkspaceContext : TheWorkspaceContext
    {
        private DetentionReporter reporter;
        private MessageDialog dlg;
        private bool isShow;

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workspace">工作空间</param>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<ContractAccountFramePage, ContractAccountPageContext>();
            //Register<BusinessProcessWizardFramePage, ContractAccountPageContext>();
        }

        #endregion Ctor

        #region Method

        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
            //if (TheApp.Current.GetIsAuthenticated())
            //    return true;

            //var onlinexts = TheApp.Current.GetAuthenticator().GetLicenseOnlines();
            //var modul = onlinexts.Where(t => t.ModuleId == "").FirstOrDefault();
            //if (modul != null)
            //{
            //    var decString = Encrypter.DecryptDES(modul.LicenseKey, "3D7B2053-A073-4624-9904-CAF85E54B842");
            //    var dic = Serializer.DeserializeFromJsonString<Dictionary<string, string>>(decString);
            //    var region = dic["region"];
            //    var czoneslist = new List<string>();
            //    var zoneslist = new List<string>();
            //    foreach (var item in region.Split(','))
            //    {
            //        czoneslist.Add(item);
            //        zoneslist.Add(item);
            //        if (item.Length >= 2)
            //        {
            //            var province = item.Substring(0, 2);
            //            if (!zoneslist.Contains(province))
            //                zoneslist.Add(province);
            //        }
            //        if (item.Length >= 4)
            //        {
            //            var city = item.Substring(0, 4);
            //            if (!zoneslist.Contains(city))
            //                zoneslist.Add(city);
            //        }
            //        if (item.Length >= 6)
            //        {
            //            var county = item.Substring(0, 6);
            //            if (!zoneslist.Contains(county))
            //                zoneslist.Add(county);
            //        }
            //    }
            //    if (!zoneslist.Contains("86"))
            //        zoneslist.Add("86");
            //    var currentzonecode = Workspace.Properties.TryGetValue("CurrentZoneCode", "");
            //    if (!string.IsNullOrEmpty(currentzonecode))
            //    {
            //        if (currentzonecode.Length <= 9 && zoneslist.Contains(currentzonecode))
            //        {
            //            return true;
            //        }
            //        if (currentzonecode.Length > 9 && zoneslist.Any(t => currentzonecode.StartsWith(t)))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;
        }

        /// <summary>
        /// 注册了一张页面，即承包台账主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(ContractAccountFramePage) });
        }

        //protected override void OnInstallComponent(object sender, InstallComponentEventArgs e)
        //{
        //    dlg = new MessageDialog();
        //    dlg.MessageGrade = eMessageGrade.Warn;
        //    dlg.Header = LanguageAttribute.GetLanguage("lang1010009");

        //    reporter = DetentionReporterDispatcher.Create(
        //        dlg.Dispatcher, c => ValidateAuthenticatedKey(), 10000, 10000, true);

        //    reporter.Start();
        //}

        private void ValidateAuthenticatedKey()
        {
            if (isShow || Workspace == null || Workspace.Message == null)
                return;

            var key = TheApp.Current.GetAuthenticator().GetAuthenticatedKey();
            var extends = TheApp.Current.GetAuthenticator().GetAuthenticatedKeyExtends();
            var args = new AuthenticatedKeyEventArgs(key, extends);
            Workspace.Message.Send(this, args);

            if (args.ReturnValue)
            {
                return;
            }
            else
            {
            }

            if (args.Parameter.IsNullOrBlank())
                dlg.Message = LanguageAttribute.GetLanguage("lang1010010");

            isShow = true;
            Workspace.Window.ShowDialog(dlg, (b, r) =>
            {
                isShow = false;

                if (b != null && !b.Value)
                {
                    Workspace.Close();
                    return;
                }

                if (b != null && b.Value)
                    TheApp.Current.RefreshAuthenticated();

                var val = TheApp.Current.GetIsAuthenticated();
                if (!val)
                {
                    reporter.MillisecondsTimeoutFirst = 1000;
                    reporter.MillisecondsTimeout = 1000;
                }
                else
                {
                    reporter.MillisecondsTimeoutFirst = 10000;
                    reporter.MillisecondsTimeout = 10000;
                }
            });
        }

        #endregion Method
    }
}