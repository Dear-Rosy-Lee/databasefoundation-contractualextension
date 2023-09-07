using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
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

        #endregion

        #region Methods

        #region Methods - Message

        protected override void OnWorkpageShown()
        {
            var codeZone = Workpage.Workspace.Properties.TryGetValue<string>("CurrentZoneCode", null);
            if (codeZone.IsNullOrBlank())
                return;

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
                }

                return null;
            });
        }

        [MessageHandler(ID = EdCore.langNavigateTo)]
        protected virtual void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            Workpage.Workspace.Properties["CurrentZoneCode"] = null;

            if (e.Object == null)
                return;

            var zone = e.Object.Object as YuLinTu.Library.Entity.Zone;
            if (zone == null)
                return;

            Workpage.Properties["CurrentZoneCode"] = zone.FullCode;
            Workpage.Workspace.Properties["CurrentZoneCode"] = zone.FullCode;
        }

        #endregion

        #endregion

    }
}
