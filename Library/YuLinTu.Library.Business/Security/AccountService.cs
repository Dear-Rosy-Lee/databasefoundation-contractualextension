using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    public class AccountService : IAccountService
    {
        public string GetCurrentUserName()
        {
            return TheApp.Current.GetWorkspaceUserName();
        }
    }
}