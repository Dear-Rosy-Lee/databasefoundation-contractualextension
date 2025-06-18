using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Appwork;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.DF.Common;
using YuLinTu.DF.Component.Mvvm;
using YuLinTu.Security;
using YuLinTu.Security.Online;
using YuLinTu.Windows;

namespace YuLinTu.Component.VectorDataDecoding
{
    public class WorkpageContext : TheWorkpageContextBase
    {
        public WorkpageContext(IWorkpage workpage) : base(workpage)
        {
            
        }

        protected override void OnWorkspaceMessageReceived(object sender, MsgEventArgs e)
        {
            base.OnWorkspaceMessageReceived(sender, e);
            //switch (e.ID)
            //{
            //    case HomesteadMsg.RefreshLand:
            //        Workpage.Message.Send(this, new RefreshEventArgs());
            //        break;
            //}
        }
        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }
       
    }
}