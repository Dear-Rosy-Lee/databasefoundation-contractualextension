using Microsoft.Scripting.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.DF;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(UploadVectorDataAfterDecodeArgument),
        Name = "上传已脱密矢量文件", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class UploadVectorDataAfterDecode : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields
        private int UploadDataLimit = 500;
        #endregion

        #region Ctor

        public UploadVectorDataAfterDecode()
        {
            Name = "上传已脱密矢量文件";
            Description = "上传已脱密矢量文件";
        }

        #endregion

        #region Methods
        private IVectorService vectorService { get; set; }    
        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as UploadVectorDataAfterDecodeArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            if (args.ResultFilePath.IsNullOrEmpty())
            {
                this.ReportError("请选择存放数据的文件夹");
                return;
            }
            if (args.ShpFilesInfo==null|| args.ShpFilesInfo.Count==0)
            {
                this.ReportError("未识别到矢量数据");
                return;
            }
            vectorService = new VectorService();
            // TODO : 任务的逻辑实现
            //读取文件夹中的矢量文件列表
            // var shps = ShpFolderDescription.GetFilesByExtensionLegacy(args.ResultFilePath);

            //int fileCount = 0;int dataCount = 0;
            //List<ShpFileDescription> files = new List<ShpFileDescription>();
            //foreach (var shp in shps )
            //{
            //    var shpInfo= ShpFolderDescription.GetShpFileDescription(shp);
            //    fileCount++; dataCount += shpInfo.DataCount;
            //    args.DataCount = string.Format("共{0}个矢量文件个数,{1}个地块", fileCount, dataCount);
            //    args.CheckInfo = args.CheckInfo + shp.ReplaceFirst(args.ResultFilePath, ".") + "  地块数量：" + shpInfo.DataCount + "  " + shpInfo.Description+"\n";
            //    files.Add(shpInfo);
            //}
            var files = args.ShpFilesInfo;
            int sucessCount = 0;int fileIndex = 0;int progess = 0;
            foreach (var item in files )
            {
                fileIndex++;
                if (item.Description.IsNotNullOrEmpty()) continue;
                List<SpaceLandEntity>upLoadData= new List<SpaceLandEntity>();   
                using (var shp = new ShapeFile())
                {
                    var err = shp.Open(item.FullPath);
                    if (!string.IsNullOrEmpty(err))
                    {
                        throw new Exception("读取地块Shape文件发生错误" + err);
                    }
                    var codeIndex = new Dictionary<string, int>();

                   var shpEnum= VectorDataProgress.ForEnumRecord<SpaceLandEntity>(shp, item.FullPath, codeIndex, 4490,"DKBM");
                    this.ReportInfomation($"开始上传脱密数据：{item.FullPath}");
                    bool isSucess;
                    foreach (var entity in shpEnum )
                    {
                        upLoadData.Add(entity);
                        if(upLoadData.Count== UploadDataLimit)
                        {
                            //上传数据
                            progess += ((100 * upLoadData.Count / files.Count) ) / item.DataCount;
                            vectorService.UpLoadVectorDataAfterDecodeToSever(upLoadData, out  isSucess);
                            if(isSucess)
                            {
                                sucessCount += upLoadData.Count;
                                this.ReportInfomation($"成功上传条数{upLoadData.Count},地块编码范围：{upLoadData[0].DKBM}~{upLoadData[upLoadData.Count-1].DKBM}");
                               
                            }
                            this.ReportProgress(progess, "上传中");
                            upLoadData.Clear(); 
                        }
                    }
                    if (upLoadData.Count != 0)
                    {
                        vectorService.UpLoadVectorDataAfterDecodeToSever(upLoadData, out isSucess);
                        if (isSucess)
                        {
                            progess += ((100 * upLoadData.Count / files.Count)) / item.DataCount;
                            sucessCount += upLoadData.Count;
                            this.ReportProgress(progess, "上传中");
                            this.ReportInfomation($"成功上传条数{upLoadData.Count},地块编码范围：{upLoadData[0].DKBM}~{upLoadData[upLoadData.Count - 1].DKBM}");
                        }
                    }
      
                    
                    this.ReportInfomation($"地块总数量：{item.DataCount}，成功上传数量：{sucessCount}");
                }
            }

           //分批上传矢量数据
            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

       


        #endregion

        #endregion
    }
}
