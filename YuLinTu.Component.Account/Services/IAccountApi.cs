using Refit;
using System.Threading.Tasks;
using YuLinTu.Component.Account.Models;

namespace YuLinTu.Component.Account.Services
{
    public interface IAccountApi
    {
        [Post("/account/login_with_info")]
        Task<AccountResult> LoginAsync([Body] LoginInfo input);

        [Get("/account/member/logout")]
        Task<AccountResult> LogoutAsync();

        [Get("/account/member/user_info_all")]
        Task<AccountResult<UserInfo>> GetUserInfoAllAsync();

        [Get("/account/member/{id}/portrait")]
        Task<byte[]> GetPortraitAsync(string id, long t);
    }
}