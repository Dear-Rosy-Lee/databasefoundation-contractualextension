using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.DF.Files;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(UploadProveFilesArgument),
        Name = "上传证明材料", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class UploadProveFiles : YuLinTu.Task
    {
        #region Properties
        internal IVectorService vectorService { get; set; }
        #endregion

        #region Fields

        #endregion

        #region Ctor

        public UploadProveFiles()
        {
            Name = "上传证明材料";
            Description = "按乡镇上传证明材料";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");
            vectorService= new VectorService();
            var args = Argument as UploadProveFilesArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现
            ProveFileEn en=new ProveFileEn();
            en.dybm = args.ZoneCode;
            en.file_name =Path.GetFileName( args.ResultFilePath);
            en.upload_time=DateTime.Now.ToLongDateTimeString();
           
            using (FileStream fs = new FileStream(args.ResultFilePath, FileMode.Open))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    en.base_str=Convert.ToBase64String(ms.ToArray());
                }
            }
            if (string.IsNullOrWhiteSpace(en.base_str))
            {
                this.ReportError("文件内容为空，请核查！");
                return;
            }
            if (en.base_str.Length % 4 != 0 || !Regex.IsMatch(en.base_str, @"^[a-zA-Z0-9\+/]*={0,3}$"))
            {
               var  message = "文件错误，请核查！" ;
                this.ReportError(message);
                return;
            }
            string msg=  vectorService.UpLoadProveFile(en,out bool sucess);
            if(!sucess) {
                this.ReportError(msg);
                return;
            }
            else
            {
                this.ReportProgress(100, "完成");
            }
           
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
