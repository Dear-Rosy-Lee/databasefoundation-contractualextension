using System;
using System.Collections.Generic;
using System.Configuration;
using Autofac;
using Newtonsoft.Json;
using Refit;
using YuLinTu.Appwork;
using YuLinTu.Component.Account.Models;
using YuLinTu.DF.Common;
using YuLinTu.Security;
using YuLinTu.Windows;

namespace YuLinTu.Component.Account.Services
{
    public class AccountService : ISecurityQuery
    {
        protected IContainer PluginContainer { get; }

        protected IAccountApi Api
        {
            get
            {
                var url = ConfigurationManager.AppSettings.TryGetValue(AppParameters.stringDefaultSecurityService, AppParameters.stringDefaultSecurityServiceValue);
                //var url = TheApp.Current.GetSystemSection().TryGetValue(AppParameters.stringDefaultSecurityService, AppParameters.stringDefaultSecurityServiceValue);
                return RefitService.For<IAccountApi>(url);
            }
        }

        protected IAccountApi GetApi(RefitSettings settings)
        {
            return RefitService.For<IAccountApi>(TheApp.Current.GetSystemSection().TryGetValue(
                AppParameters.stringDefaultSecurityService,
                AppParameters.stringDefaultSecurityServiceValue), settings);
        }

        public bool Active(Guid sessionCode)
        {
            throw new NotImplementedException();
        }

        public int ChangeHeadPortrait(Guid sessionCode, string imageBase64)
        {
            throw new NotImplementedException();
        }

        public int ChangeNickName(Guid sessionCode, string nickName)
        {
            throw new NotImplementedException();
        }

        public int ChangeStatus(Guid sessionCode, eObjectStatus status)
        {
            throw new NotImplementedException();
        }

        public string GetHeadPortrait(Guid sessionCode)
        {
            var user = GetUserInfo(sessionCode);
            if (user is null) return null;
            var result = GetApi(new RefitSettings
            {
                ContentSerializer = new StreamSerializer()
            }).GetPortraitAsync(user.Id, DateTime.Now.Ticks).Result;
            var base64 = Convert.ToBase64String(result);
            //var aa = BitmapFrame.Create(new MemoryStream(Convert.FromBase64String(base64)));
            //var sec = TheApp.Current.GetUserSection(user.Name);
            return base64;
        }

        public SecurityRange GetModuleRange(Guid sessionCode, string scope)
        {
            throw new NotImplementedException();
        }

        public SecurityRoots GetModuleRoots(Guid sessionCode)
        {
            throw new NotImplementedException();
        }

        public string GetNickName(Guid sessionCode)
        {
            throw new NotImplementedException();
        }

        public List<RoleAtom> GetRoles(Guid sessionCode)
        {
            throw new NotImplementedException();
        }

        public SecurityRange GetScopeRange(Guid sessionCode, string module)
        {
            throw new NotImplementedException();
        }

        public SecurityRoots GetScopeRoots(Guid sessionCode)
        {
            throw new NotImplementedException();
        }

        public ObjectInfomation GetSession(Guid sessionCode)
        {
            throw new NotImplementedException();
        }

        public SecurityObjectSettings GetSettings(Guid sessionCode, string category, string key)
        {
            var user = GetUserInfo(sessionCode);
            if (user is null) return null;

            var setting = new SecurityObjectSettings
            {
                Category = category,
                Key = key,
                Version = DateTime.Now
            };
            switch (key)
            {
                case "HeadPortrait":
                    setting.Value = GetHeadPortrait(sessionCode);
                    break;

                case "UserName":
                    setting.Value = user.Name;
                    break;

                case "NickName":
                    setting.Value = user.Nickname;
                    break;

                default:
                    break;
            }

            return setting;
        }

        public List<SecurityObjectSettings> GetSettings(Guid sessionCode, List<ObjectSettingsPair> pairs)
        {
            throw new NotImplementedException();
        }

        public bool HasObject(string name, string password)
        {
            throw new NotImplementedException();
        }

        public bool HasPermission(Guid sessionCode, string scope, string module)
        {
            throw new NotImplementedException();
        }

        public List<bool> HasPermissions(Guid sessionCode, List<PermissionAtom> pers)
        {
            throw new NotImplementedException();
        }

        public List<bool> HasPermissionsWithinOrContains(Guid sessionCode, List<PermissionAtom> pers)
        {
            throw new NotImplementedException();
        }

        public bool HasPermissionWithinOrContains(Guid sessionCode, string scope, string module)
        {
            throw new NotImplementedException();
        }

        public bool HasSession(Guid sessionCode)
        {
            return sessions.Contains(sessionCode);
        }

        private static HashSet<Guid> sessions = new HashSet<Guid>();

        public Guid? Login(string name, string password, string application)
        {
            password = Encrypter.DecryptDES(password.Trim());
            var result = Api.LoginAsync(new LoginInfo(name, password)).Result;
            if (result.Success)
            {
                var jsonString = result.Data.ToString();
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
                string region = jsonObject.region;
                AppGlobalSettings.Current[Parameters.UserName] = name;
                if (!region.IsNullOrEmpty())
                {
                    AppGlobalSettings.Current[Parameters.RegionName] = region;
                    CommonSettingDefine.Instance.CurrentZoneCode = "512022"; //region;//导航树的根地域、具有权限的地域
                   
                }
                //Parameters.Region = region;
                string token = jsonObject.token;
                Guid session;
                if (Guid.TryParse(token, out session))
                {
                    AppGlobalSettings.Current[Parameters.TokeName] = session;
                    //Parameters.Token = session;
                    return session;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception(result.Msg.Replace("用户", "用户名"));
            }
        }

        public bool Logout(Guid sessionCode)
        {
            var result = Api.LogoutAsync().Result;
            // Parameters.Token = Guid.Empty;
            AppGlobalSettings.Current[Parameters.TokeName] = Guid.Empty;
            return result.Success;
        }

        public int SetSettings(Guid sessionCode, string category, string key, string value)
        {
            throw new NotImplementedException();
        }

        public int SetSettings(Guid sessionCode, List<ObjectSettingsPair> pairs)
        {
            throw new NotImplementedException();
        }

        public UserInfo GetUserInfo(Guid sessionCode)
        {
            var result = Api.GetUserInfoAllAsync().Result;
            if (result.Success)
            {
                return result.Data;
            }
            return null;
        }
    }
}