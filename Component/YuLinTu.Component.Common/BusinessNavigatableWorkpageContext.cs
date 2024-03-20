/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component
{
    public abstract class BusinessNavigatableWorkpageContext : TheNavigatableWorkpageContext
    {
        #region Ctor

        public BusinessNavigatableWorkpageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion Ctor

        #region Methods

        #region Methods - Message

        /// <summary>
        /// 设置改变时会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSettingsChanged(object sender, SettingsProfileChangedEventArgs e)
        {
            if (Navigator == null)
            {
                return;
            }
            if (e.Profile.Name == TheBns.stringDataSourceNameChangedMessageKey)
            {
                Navigator.Reload();
                while (Workpage.Page.CloseDialog() != null) ;
            }
            else if (e.Profile.Name == "CURRENTROOTCHANGE")
            {
                Navigator.Reload();
            }
        }

        protected override void OnNavigatorFirstLoadComplete(object sender, MsgEventArgs<Navigator> e)
        {
        }

        protected override void OnWorkpageShown()
        {
            var codeZone = Workpage.Workspace.Properties.TryGetValue<string>("CurrentZoneCode", null);
            if (Navigator == null)
            {
                return;
            }
            if (codeZone.IsNullOrBlank())
            {
                if (Navigator.SelectedItem != null)
                    Navigator.Reload();
                return;
            }
            var codeZonePage = Workpage.Properties.TryGetValue<string>("CurrentZoneCode", null);
            if (codeZone == codeZonePage)
                return;

            Navigator.Expand(items =>
            {
                foreach (var item in items)
                {
                    var obj = item.Object as YuLinTu.Library.Entity.Zone;
                    if (obj == null)
                        continue;
                    if (codeZone.Equals(obj.FullCode))
                        return new NavigateItem() { Object = new MetroTreeListViewExpandTarget(item) };
                    if (codeZone.StartsWith(obj.FullCode))
                        return item;
                    if (obj.FullCode == "86")
                        return item;
                }

                return null;
            });
        }

        [MessageHandler(ID = EdCore.langNavigateSelectedItemChanged)]
        protected virtual void OnNavigateSelectedItemChanged(object sender, NavigateSelectedItemChangedEventArgs e)
        {
            Workpage.Properties["CurrentZoneCode"] = null;
            Workpage.Workspace.Properties["CurrentZoneCode"] = null;
            Workpage.Properties["CurrentZone"] = null;
            Workpage.Workspace.Properties["CurrentZone"] = null;

            if (e.Object == null)
                return;

            var zone = e.Object.Object as YuLinTu.Library.Entity.Zone;
            if (zone == null)
                return;

            Workpage.Properties["CurrentZoneCode"] = zone.FullCode;
            Workpage.Workspace.Properties["CurrentZoneCode"] = zone.FullCode;
            Workpage.Properties["CurrentZone"] = zone;
            Workpage.Workspace.Properties["CurrentZone"] = zone;
        }

        [MessageHandler(ID = EdCore.langNavigateTo)]
        protected virtual void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
        }

        #endregion Methods - Message

        #endregion Methods
    }
}