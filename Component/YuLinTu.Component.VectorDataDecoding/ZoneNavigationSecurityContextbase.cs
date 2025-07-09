using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.DF.Controls.Messages;
using YuLinTu.DF.Data;
using YuLinTu.DF.Zones;
using YuLinTu.DF.Component.Navigation;
using YuLinTu.DF.Common;
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using YuLinTu.Security.Online;
using System.IO;
using YuLinTu.Security;
using YuLinTu.Component.VectorDataDecoding.Core;
using System.Linq;

namespace YuLinTu.Component.VectorDataDecoding
{
  
    public class ZoneNavigationSecurityContextbase<T> : ZoneNavigationContextbase where T: PageFrame
    {
        #region Properties

        public new T PageContent
        { get { return base.PageContent as T; } }

        private IVectorService vectorService { get;set; }
        public override bool IsConnectDB { get => base.IsConnectDB; set => base.IsConnectDB = false; }
        #endregion Properties

        #region Fields

        #endregion Fields

        #region Ctor

        public ZoneNavigationSecurityContextbase(IWorkpage workpage)
            : base(workpage)
        {
            IsConnectDB = false;
            Constants.ZonesCodes = GetSecurityZoneCode();
            CommonSettingDefine.Instance.CurrentZoneCode = GetRootCode(Constants.ZonesCodes); //region;//导航树的根地域、具有权限的地域                               
            vectorService=new VectorService();
        }

        private string GetRootCode(List<string> zones)
        {
            //
            int[] length = { 2, 4, 6, 9, 12 };
            if (zones.Contains("86")) return "86";
            int mixLength = zones[0].Length;
            zones.ForEach(t =>
            {
                if (t.Length < mixLength)
                {
                    mixLength = t.Length;
                }
            });
            length = length.Where(t => t <= mixLength).ToArray();
            string rootCode = string.Empty;
            for (int i = (length.Length - 1); i >= 0; i--)
            {
                var codes = zones.Select(t => t.Substring(0, length[i])).Distinct().ToArray();
                if (codes.Count() == 1)
                {
                    rootCode = codes[0];
                    break;
                }
            };
            return rootCode;
        }

        private List<string> GetSecurityZoneCode()
        {
            List<string> ZoneCodes=new List<string>();
            var OnlineKeyFileName = TheApp.Current.GetAuthenticateOnlineKeyFileName();
            if (File.Exists(OnlineKeyFileName))
            {

                try
                {


                    var ao = Serializer.DeserializeFromXmlFile<AuthenticateOnline>(OnlineKeyFileName);
                   
                    foreach (var item in ao.Licenses)
                    {
                        var decString = Encrypter.DecryptDES(item.LicenseKey, Constants.moduleUniqueKey);
                        var dic = Serializer.DeserializeFromJsonString<Dictionary<string, string>>(decString);
                        var region = dic["region"];
                        ZoneCodes.AddRange(region.Split(','));
                        
                    }

                }
                catch (Exception ex)
                {
                    Tracker.WriteLine(new TrackerObject()
                    {
                        EventID = EdSecurity.langReadAuthenticatedKeyException,
                        Description = string.Format(new EdSecurity().GetName(
                            EdSecurity.langReadAuthenticatedKeyException), ex),
                        Grade = eMessageGrade.Warn,
                        Source = typeof(Authenticate).FullName,
                    });

                }



            }
            else
            {
                //throw new Exception("未获授权");
            }



            //若多个地域取其上级地域
            return ZoneCodes;
        }

        #endregion Ctor

        #region Methods

        #region Methods - Override

        public override void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.NavigateTo(sender, e);

            //var args = new RefreshEventArgs();
            //Workpage.Message.Send(this, args);
        }

        protected override int ShowContentCountByZoneCode(string zoneCode)
        {
            return vectorService.GetBatchsCountByZoneCode(zoneCode,Constants.ClientType);
        }

        #endregion Methods - Override

        #endregion Methods
    }
}