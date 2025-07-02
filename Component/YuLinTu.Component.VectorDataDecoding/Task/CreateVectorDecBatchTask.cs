using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Security;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(CreateVectorDecBatchTaskArgument),
        Name = "创建图斑匹配批次", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class CreateVectorDecBatchTask : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields
        private string baseUrl = Constants.baseUrl;
        private Dictionary<string, string> AppHeaders;
        private string methold = Constants.Methold_CreateVectorDecTask;// "/stackcloud/api/open/api/dynamic/onlineDecryption/task/add";
        #endregion

        #region Ctor

        public CreateVectorDecBatchTask()
        {
            Name = "创建创建图斑匹配批次";
            Description = "选择乡镇地域后创建创建图斑匹配批次";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as CreateVectorDecBatchTaskArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            if (args.ZoneCode.Length < 9)
            {
                this.ReportError("无法创建矢量数据脱密任务！请选择乡镇或村级地域后创建任务。");
                return;
            }
            AppHeaders = new Dictionary<string, string>();//应该是登录以后通过appID获取key??
            AppHeaders.Add(Constants.appidName, Constants.appidVaule);
            AppHeaders.Add(Constants.appKeyName, Constants.appKeyVaule);
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            // TODO : 任务的逻辑实现
            string url = Constants.baseUrl+ Constants.Methold_CreateVectorDecTask;
            Dictionary<string,string>body=new Dictionary<string,string>();
            body.Add("dybm", args.ZoneCode);
            //var clientID = new Authenticate().GetApplicationKey();

            body.Add("client_id", Constants.client_id);
            body.Add("upload_batch_name", args.BatchName);
            //body.Add("remarks", "测试");
            //var jsonData = JsonSerializer.Serialize(body);
            var en = apiCaller.PostResultAsync<BatchTaskJsonEn>(url, AppHeaders, JsonSerializer.Serialize(body));
            if(en!=null)
            {
                string info = "创建矢量数据处理任务成功！任务批次编码为：" + en.upload_batch_num;
                this.ReportInfomation(info);
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
