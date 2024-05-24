using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using System.Diagnostics;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using System.Reflection;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    /// <summary>
    /// 质检、加密、压缩汇交成果任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "加密汇交成果",
        Gallery = "汇交成果处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class QualityCompressionData : Task
    {
        #region Ctor

        public QualityCompressionData()
        {
            Name = "质检、加密、压缩汇交成果";
            Description = "质检、加密、压缩汇交成果";
        }

        #endregion Ctor

        #region Fields

        private QualityCompressionDataArgument argument;

        #endregion Fields

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
            var password = TheApp.Current.GetSystemSection().TryGetValue(
                Parameters.stringZipPassword,
                Parameters.stringZipPasswordValue);
            var sourceFolder = argument.CheckFilePath;
            var zipFilePath = $"{argument.ResultFilePath}\\{Path.GetFileName(sourceFolder)}.zip";
            try
            {
                //进行质检
                bool flag = ExcuteQuality(sourceFolder);
                if (flag == false)
                {
                    this.ReportError(string.Format("数据检查时出错!具体错误请查看质检错误报告"));
                }
                else
                {
                    using (var fsOut = File.Create(zipFilePath))
                    {
                        using (var zipStream = new ZipOutputStream(fsOut))
                        {
                            zipStream.SetLevel(5); // 压缩级别，0-9，9为最大压缩
                            zipStream.Password = password;
                            CompressFolder(sourceFolder, zipStream, sourceFolder.Length);
                        }
                    }
                }
                //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
                return;
            }

            this.ReportProgress(100);
            this.ReportInfomation("检查完成。");
        }

        #endregion Method - Override

        #region Method - Private

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

        private void CompressAndEncryptFolder(string sourceFolder, string zipFilePath, string password)
        {
            FastZip fastZip = new FastZip();
            fastZip.Password = password;
            fastZip.CreateZip(zipFilePath, sourceFolder, true, "");
        }

        private static void CompressFolder(string sourceFolder, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(sourceFolder);

            foreach (string file in files)
            {
                var fileInfo = new FileInfo(file);

                // 创建压缩文件条目
                string entryName = file.Substring(folderOffset);
                entryName = ZipEntry.CleanName(entryName); // 移除任何相对路径
                var newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fileInfo.LastWriteTime;
                newEntry.Size = fileInfo.Length;

                zipStream.PutNextEntry(newEntry);

                // 将文件内容写入zip流
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(file))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            // 获取文件夹下所有子文件夹
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }

            // 如果文件夹为空，则创建空文件夹条目
            if (files.Length == 0 && folders.Length == 0)
            {
                string entryName = sourceFolder.Substring(folderOffset) + "/";
                entryName = ZipEntry.CleanName(entryName); // 移除任何相对路径
                var newEntry = new ZipEntry(entryName);
                zipStream.PutNextEntry(newEntry);
                zipStream.CloseEntry();
            }
        }

        private bool ExcuteQuality(string arg)
        {
            // 外部exe程序路径
            string assemblyLocation = Assembly.GetEntryAssembly().Location;
            string installPath = Path.GetDirectoryName(assemblyLocation);
            string exePath = $"{installPath}\\bin\\StartWithExternal.exe";
            if (!File.Exists(exePath))
            {
                this.ReportError("当前exe可执行文件路径不存在");
                return false;
            }

            // 创建进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exePath;
            startInfo.Arguments = $"{arg}"; // 参数之间用空格分隔

            // 可选：隐藏执行程序的窗口
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // 创建并启动进程
            Process process = new Process();
            process.StartInfo = startInfo;
            var date = DateTime.Now.ToString("MM月dd日HH时mm分");
            process.Start();

            // 可选：等待进程执行完毕
            process.WaitForExit();

            //返回质检结果
            return ReturnResult(arg, date);
        }

        private bool ReturnResult(string pathName, string date)
        {
            //拼写检查路径
            string fileName = $"{pathName + "检查结果"}\\自定义检查检查信息{date}\\检查结果记录(1).txt";
            string fileContent = File.ReadAllText(fileName);
            return !fileContent.Contains("错误");
        }

        #endregion Method - Private

        #endregion Methods
    }
}