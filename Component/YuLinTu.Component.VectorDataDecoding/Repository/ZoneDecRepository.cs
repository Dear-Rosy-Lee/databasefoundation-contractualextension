using Autofac;
using AutoMapper;
using RTools_NTS.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data;
using YuLinTu.DF;
using YuLinTu.DF.Data;
using YuLinTu.DF.DependencyInjection;
using YuLinTu.DF.Repositories;
using YuLinTu.DF.Zones;
using YuLinTu.Spatial;

namespace YuLinTu.Component.VectorDataDecoding.Repository
{
    public class ZoneDecRepository : YltRepository<XZQH_XZDY, Guid>, IZoneRepository
    {

        private string baseUrl = Constants.baseUrl;// ConfigurationManager.AppSettings.TryGetValue<string>("DefaultBusinessAPIAdress", "https://api.yizhangtu.com");
        private Dictionary<string, string> AppHeaders;
        protected  IMapper mapper;
        #region Methods


        #endregion
        public ZoneDecRepository(IDataSource ds) : base(ds)
        {
            AppHeaders=new Dictionary<string, string>();//应该是登录以后通过appID获取key??
            AppHeaders.Add(Constants.appidName, Constants.appidVaule);
            AppHeaders.Add(Constants.appKeyName, Constants.appKeyVaule);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ZoneJsonEn, XZQH_XZDY>();
            });

            mapper = config.CreateMapper();
        }
       

        public bool AnyByFullName(string uplevelName, string zoneName)
        {
            throw new NotImplementedException();
        }

        public bool AnyByFullName(string uplevelName, string zoneName, Guid id)
        {
            throw new NotImplementedException();
        }

        public bool AnyChildren(string zoneCode, eLevelOption options = eLevelOption.SelfAndSubs)
        {
            string url = string.Empty;
            url = baseUrl + "/stackcloud/api/open/api/xzdy/code/child/count";
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("code", zoneCode);
            var en = apiCaller.GetResultStringAsync(url, AppHeaders, parms);
             int.TryParse(en,out int count);
            return count > 0;

        }

        public override Expression<Func<XZQH_XZDY, bool>> FilterExpression(string filter)
        {
            throw new NotImplementedException();
        }

        public List<XZQH_XZDY> GetByLevel(string zoneCode, bool friendly = true, params ZoneLevel[] levels)
        {
            throw new NotImplementedException();
        }

        public XZQH_XZDY GetByZoneCode(string zoneCode, bool friendly = true)
        {
            string url = string.Empty;
            url = baseUrl + "/stackcloud/api/open/api/xzdy/detail";
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("code", zoneCode);
            var en = apiCaller.GetResultAsync<ZoneJsonEn>(url, AppHeaders, parms);
            var zone=  mapper.Map<ZoneJsonEn, XZQH_XZDY>(en);
            return zone;
        }
        //[Get("/account/member/logout")]
        public List<XZQH_XZDY> GetChildrenByZoneCode(string zoneCode, eLevelOption options = eLevelOption.SelfAndSubs, bool friendly = true)
        {
            string url = string.Empty;
             if(options== eLevelOption.SelfAndSubs)
            {
                url = baseUrl + "stackcloud/api/open/api/xzdy/children/tree/include/self";
            }else if(options == eLevelOption.Self)
            {
                url = baseUrl + "/stackcloud/api/open/api/xzdy/children";
            }
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();         
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("code", zoneCode);
            var en = apiCaller.GetResultListAsync<ZoneJsonEn>(url, AppHeaders, parms);
            var zones = mapper.Map<List<ZoneJsonEn>, List<XZQH_XZDY>>(en).ToList();
            return zones;
        }

        public XZQH_XZDY GetCountyZone()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<XZQH_XZDY, string>> GetKey()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, bool> GetParentAndChildrenCodeAndContainsShapeByZoneCode(string codeZone)
        {
            throw new NotImplementedException();
        }

        public List<XZQH_XZDY> GetParentsByZoneCode(string zoneCode, eLevelOption options = eLevelOption.SelfAndSubs, bool friendly = true)
        {
            string url = string.Empty;
            url = baseUrl + "/stackcloud/api/open/api/xzdy/parents";
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("code", zoneCode);
            var en = apiCaller.GetResultListAsync<ZoneJsonEn>(url, AppHeaders, parms,true);
            var zones = mapper.Map<List<ZoneJsonEn>, List<XZQH_XZDY>>(en).ToList();
            return zones;
        }

        public SpatialReference GetSpatialReference()
        {
            throw new NotImplementedException();
        }
    }
}
