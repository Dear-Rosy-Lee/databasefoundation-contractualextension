using Ionic.Zip;
using System;
using System.IO;
using YuLinTu;
using YuLinTu.Library.Log;

namespace YuLinTu.Component.UpdateShpByLandNumberTask
{
    /// <summary>
    /// 农业部数据交换任务导入
    /// </summary>
    [TaskDescriptor(Name = "根据地块编码更新图斑", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Component.BatchDataBaseTask;component/Resources/import16.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Component.BatchDataBaseTask;component/Resources/import24.png")]
    public class UpdateShpByLandNumberData : Task
    {
        public UpdateShpByLandNumberData()
        {
            Name = "根据地块编码更新图斑";
            Description = "根据地块编码更新图斑";
        }
        #region Fields

        private UpdateShpByLandNumberDataArgument argument;

        #endregion Fields

        #region Properties

        private string ErrorInfo { get; set; }

        #endregion Properties

        #region Methods

        #region Method - Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证检查数据参数...");
            this.ReportInfomation("开始验证检查数据参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
            {
                this.ReportProgress(100);
                return;
            }

            this.ReportProgress(1, "开始检查...");
            this.ReportInfomation("开始检查...");
            System.Threading.Thread.Sleep(200);

            //TODO 加密方式
            //var password = TheApp.Current.GetSystemSection().TryGetValue(
            //    Parameters.stringZipPassword,
            //    Parameters.stringZipPasswordValue);
            var sourceFolder = argument.UpdateFilePath.Substring(0, argument.UpdateFilePath.Length - 4);
            
            try
            {
                //进行质检
                var dcp = new DataCheckProgress();
                dcp.DataArgument = argument;

                bool falg = dcp.Check();
                ErrorInfo = dcp.ErrorInfo;
                if (falg == false)
                {
                    this.ReportError(ErrorInfo);
                    Log.WriteError(this, "提示", ErrorInfo);
                    return;
                }
                else
                {
                    var path = dcp.CreateLog();
                    dcp.WriteLog(path, new KeyValueList<string, string>
            {
                new KeyValue<string, string>("", "已通过数据质检!\r")
            });
                    this.ReportProgress(100);
                    this.ReportInfomation("更新成功。");
                }
                //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "OnGo(数据更新失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据更新时出错!"));
                return;
            }
        }

        #endregion Method - Override

        #region Method - Private
        private bool ValidateArgs()
        {
            var args = Argument as UpdateShpByLandNumberDataArgument;

            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            argument = args;
            try
            {
                if (args.UpdateFilePath.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("待更新数据文件路径为空，请重新选择"));
                    return false;
                }
                if (args.ResultFilePath.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("结果路径为空，请重新选择"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ValidateArgs(参数合法性检查失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            this.ReportInfomation(string.Format("检查数据参数正确。"));
            return true;
        }

       
        #endregion Method - Private

        #endregion Methods
    }
}
