using YuLinTu.Appwork;
using YuLinTu.DF.Component.Mvvm;
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