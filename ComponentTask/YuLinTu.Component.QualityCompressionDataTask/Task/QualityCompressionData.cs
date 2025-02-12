using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using YuLinTu;
using YuLinTu.Library.Log;
using NetTopologySuite.Triangulate;
using System.Diagnostics;


namespace YuLinTu.Component.QualityCompressionDataTask
{
    /// <summary>
    /// 质检、加密、压缩矢量数据成果任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "加密矢量数据成果",
        Gallery = "矢量数据成果处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class QualityCompressionData : Task
    {
        #region Ctor

        public QualityCompressionData()
        {
            Name = "质检、加密、压缩矢量数据成果";
            Description = "质检、加密、压缩矢量数据成果";
        }

        #endregion Ctor

        #region Fields

        private QualityCompressionDataArgument argument;

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
            var sourceFolder = argument.CheckFilePath.Substring(0, argument.CheckFilePath.Length - 4);
            var zipFilePath = $"{argument.ResultFilePath}\\{Path.GetFileName(sourceFolder)}.zip";
            //进行质检
            var dcp = new DataCheckProgress();
            dcp.DataArgument = argument;
            if (DataCheck())
            {
                try
                {
                    CompressFolder(sourceFolder, zipFilePath);
                    var path = dcp.CreateLog();
                    dcp.WriteLog(path, new KeyValueList<string, string>
                    {
                        new KeyValue<string, string>("", "已通过数据质检!\r")
                    });
                    this.ReportProgress(100);
                    this.ReportInfomation("数据检查、加密成功。");
                    //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
                }
                catch (Exception ex)
                {
                    Library.Log.Log.WriteException(this, "OnGo(数据压缩失败!)", ex.Message + ex.StackTrace);
                    this.ReportError(string.Format("数据压缩时出错!" + ex.Message));
                }
            }
        }

        #endregion Method - Override

        #region Method - Private

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <returns></returns>
        private bool ValidateArgs()
        {
            var args = Argument as QualityCompressionDataArgument;

            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            argument = args;
            try
            {
                if (args.CheckFilePath.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("待检查数据文件路径为空，请重新选择"));
                    return false;
                }
                if (args.ResultFilePath.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("压缩文件路径为空，请重新选择"));
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

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFile">待压缩目录</param>
        /// <param name="zipStream">压缩文件</param>
        private static void CompressFolder(string sourceFile, string zipFilePath)
        {
            var fileInfo = new FileInfo(sourceFile);
            string[] matchingFiles = Directory.GetFiles(fileInfo.DirectoryName, $"{fileInfo.Name}.*", SearchOption.AllDirectories);

            using (var fsOut = File.Create(zipFilePath))
            {
                using (var zipStream = new ZipOutputStream(fsOut))
                {
                    zipStream.SetLevel(5); // 压缩级别，0-9，9为最大压缩
                    zipStream.Password = Parameters.Region + "Ylt@dzzw";
                    foreach (string matchingFile in matchingFiles)
                    {
                        var matchingFileInfo = new FileInfo(matchingFile);
                        string matchingFileInfoName = matchingFileInfo.Name;
                        string matchingFileEntryName = matchingFileInfoName; // 移除任何相对路径
                        var newEntry = new ZipEntry(matchingFileEntryName);
                        newEntry.DateTime = matchingFileInfo.LastWriteTime;
                        newEntry.Size = matchingFileInfo.Length;
                        zipStream.PutNextEntry(newEntry);
                        // 将文件内容写入zip流
                        byte[] buffer = new byte[4096];
                        using (FileStream streamReader = File.OpenRead(matchingFile))
                        {
                            StreamUtils.Copy(streamReader, zipStream, buffer);
                        }
                    }
                    zipStream.CloseEntry();
                }
            }
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        /// <returns></returns>
        private bool DataCheck()
        {
            try
            {
                //进行质检
                var dcp = new DataCheckProgress();
                dcp.DataArgument = argument;

                bool falg = dcp.Check();
                ErrorInfo = dcp.ErrorInfo;
                this.ReportProgress(10);
                if (falg == false)
                {
                    this.ReportError(ErrorInfo);
                    Log.WriteError(this, "提示", ErrorInfo);
                    return false;
                }
                else
                {
                    this.ReportInfomation("数据检查通过。");
                }
                //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
                return false;
            }
            return true;
        }

        #endregion Method - Private

        #endregion Methods
    }
}