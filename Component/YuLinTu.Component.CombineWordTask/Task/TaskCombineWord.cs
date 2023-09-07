using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.CombineWordTask
{
    [TaskDescriptor(IsLanguageName = false, Name = "合并Word",
        Gallery = "成果资料数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskCombineWord : Task
    {
        #region Fields

        protected const char SPLIT = '-';

        protected virtual string APPEND_FILE { get; set; } = "地块示意图";

        protected const string LANDPARCEL_PREFIX = "DKSYT";

        protected const string WORD_PATTERN = "*.doc";

        protected TaskCombineWordArgument _Args;

        protected readonly ToolProgress _ToolProgress;//进度条

        #endregion Fields

        #region Ctor

        public TaskCombineWord()
        {
            Name = "合并Word";
            Description = "合并Word";

            _ToolProgress = new ToolProgress();
            _ToolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(ToolProgress_OnPostProgress);
        }

        #endregion Ctor

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证合并Word参数...");
            this.ReportInfomation("开始验证合并Word参数...");

            if (!ValidateArgs())
            {
                this.ReportProgress(100);
                return;
            }

            int combinedCount = 0;
            this.ReportProgress(1, "开始合并...");
            this.ReportInfomation("开始合并...");

            try
            {
                var sourceDI = new DirectoryInfo(_Args.SourceFolder);
                var files = sourceDI.GetFiles(WORD_PATTERN, SearchOption.AllDirectories);

                var sourceDic = GetSourceDictionary(files);
                var appendDic = GetAppendDictionayr(files);

                _ToolProgress.InitializationPercent(sourceDic.Count, 90, 10);
                foreach (var item in sourceDic)
                {
                    if (!appendDic.ContainsKey(item.Key))
                    {
                        this.ReportWarn($"{item.Value.Name}未匹配到{APPEND_FILE}，已略过。");
                        continue;
                    }
                    try
                    {
                        WordTool.CombineWord(item.Value.FullName, appendDic[item.Key].FullName, Path.Combine(_Args.SaveFolder, item.Value.Name), _Args.KeepSourceFormat);
                        combinedCount++;
                    }
                    catch (Exception ex)
                    {
                        this.ReportWarn($"{item.Value.Name}与{appendDic[item.Key].Name}合并失败：{ex}");
                    }

                    _ToolProgress.DynamicProgress($"{item.Value.Name}与{appendDic[item.Key].Name}合并完成。");
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Word合并失败!", ex.ToString());
                this.ReportProgress(100);
                this.ReportError(string.Format("Word合并出错!"));
                return;
            }

            this.ReportProgress(100);
            this.ReportInfomation($"合并完成，共计合并{combinedCount}个。");
        }

        #endregion Methods - Override

        protected virtual Dictionary<int, FileInfo> GetSourceDictionary(FileInfo[] fileInfos)
        {
            throw new NotImplementedException();
        }

        protected virtual Dictionary<int, FileInfo> GetAppendDictionayr(FileInfo[] fileInfos)
        {
            throw new NotImplementedException();
        }

        #region Methods - Private

        private bool ValidateArgs()
        {
            _Args = Argument as TaskCombineWordArgument;
            if (_Args.SourceFolder.IsNullOrEmpty() || !Directory.Exists(_Args.SourceFolder))
            {
                this.ReportError("请选择源数据路径");
                return false;
            }
            if (_Args.SaveFolder.IsNullOrEmpty() || !Directory.Exists(_Args.SaveFolder))
            {
                this.ReportError("请选择数据保存路径");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 进度提示
        /// </summary>
        protected virtual void ToolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
            this.ReportInfomation(info);
        }

        #endregion Methods - Private

        #endregion Methods
    }
}