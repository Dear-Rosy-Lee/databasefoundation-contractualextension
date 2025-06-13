using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data;
using YuLinTu.DF.Data;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(UploadVectorDataToSeverArgument),
        Name = "UploadVectorDataToSever", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class UploadVectorDataToSever : YuLinTu.Task
    {
        #region Properties
        protected IDbContext DbContext { get => Db.GetInstance(); }
        private string baseUrl = Constants.baseUrl;
        private Dictionary<string, string> AppHeaders;
        private string methold = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/upload";
        #endregion

        #region Fields

        #endregion

        #region Ctor

        public UploadVectorDataToSever()
        {
            Name = "数据送审";
            Description = "将矢量数据上传至服务器，开始脱密任务";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as UploadVectorDataToSeverArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现
            //读取数据  分页读取
            var query = DbContext.CreateQuery<SpaceLandEntity>().Where(t => t.BatchCode.Equals(args.BatchCode));
            int datacount = query.Count();
            int pageSize = 100;
            int pageIndex = 0;
            int dataIndex = 0;
            AppHeaders = new Dictionary<string, string>();//应该是登录以后通过appID获取key??
            AppHeaders.Add(Constants.appidName, Constants.appidVaule);
            AppHeaders.Add(Constants.appKeyName, Constants.appKeyVaule);
            ApiCaller apiCaller = new ApiCaller();
   
            string url = baseUrl + methold;
            int progess = 0;
            while (true)
            {
                apiCaller.client = new HttpClient();
                if (dataIndex >= datacount)
                {
                    break;
                }
                var entityList = query.Paging("DKBM", eOrder.Ascending, pageIndex, pageSize) as List<SpaceLandEntity>;
                dataIndex = dataIndex + entityList.Count;

                if (entityList.Count == 0)
                {
                    break;
                }
                pageIndex++;
             
                try
                {
                 
                    List<LandJsonEn> landJsonEntites=new List<LandJsonEn>();
                    entityList.ForEach(land =>
                    {
                        LandJsonEn jsonEn = new LandJsonEn();
                        jsonEn.dybm = land.DKBM.Substring(0, 14);
                        jsonEn.upload_batch_num = args.BatchCode;
                        jsonEn.business_identification = land.DKBM;
                        jsonEn.data_type = "1";
                        jsonEn.original_geometry_data = land.Shape.AsText();
                        jsonEn.original_geometry_data = EncrypterSM.EncryptSM4(jsonEn.original_geometry_data,Constants.Sm4Key);
                        //数据加密
                        landJsonEntites.Add(jsonEn);

                    });
                    
                    var jsonData = JsonSerializer.Serialize(landJsonEntites);
                    var en = apiCaller.PostDataAsync(url, AppHeaders, jsonData);
                    //数据上传至服务器
                    entityList.Clear();
                    progess += (dataIndex * 100 / datacount) ;
                    this.ReportProgress(progess, "已送审数据条数："+dataIndex);
                }
                catch (Exception ex)
                {
                    
                    var msg = ex.ToString();
                    var erroinfo = "送审数据失败：" + msg;
                    throw new Exception(erroinfo);
                }


            }
            //加密
            apiCaller.client.Dispose();
            //上传到服务器

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
