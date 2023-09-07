using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.CombineWordTask
{
    [TaskDescriptor(IsLanguageName = false, Name = "合并WORD",
     Gallery = "成果资料数据处理",
     UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
     UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskCombineWordGroup : TaskGroup
    {
        #region Fileds

        private TaskCombineWordGroupArgument _Args;

        private eZoneLevel _ZoneLevel;

        private eCombineWordType _CombineWordType;

        #endregion Fileds

        #region Ctor

        public TaskCombineWordGroup()
        {
            Name = "合并WORD";
            Description = "合并WORD";
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
            this.Clear();

            var folderDic = new Dictionary<string, string>();
            var sourceDI = new DirectoryInfo(_Args.SourceFolder);
            switch (_ZoneLevel)
            {
                case eZoneLevel.Group:
                    folderDic.Add(sourceDI.FullName, _Args.SaveFolder);
                    break;

                case eZoneLevel.Village:
                    foreach (var item in sourceDI.GetDirectories())
                    {
                        folderDic.Add(item.FullName, Path.Combine(_Args.SaveFolder, item.Name));
                    }
                    break;

                case eZoneLevel.Town:
                    foreach (var item1 in sourceDI.GetDirectories())
                    {
                        foreach (var item in item1.GetDirectories())
                        {
                            folderDic.Add(item.FullName, Path.Combine(_Args.SaveFolder, item1.Name, item.Name));
                        }
                    }
                    break;

                case eZoneLevel.County:
                    foreach (var item1 in sourceDI.GetDirectories())
                    {
                        foreach (var item2 in item1.GetDirectories())
                        {
                            foreach (var item in item2.GetDirectories())
                            {
                                folderDic.Add(item.FullName, Path.Combine(_Args.SaveFolder, item1.Name, item2.Name, item.Name));
                            }
                        }
                    }
                    break;

                case eZoneLevel.City:
                case eZoneLevel.Province:
                case eZoneLevel.State:
                    throw new NotImplementedException();

                default:
                    break;
            }
            foreach (var item in folderDic)
            {
                TaskCombineWord task = null;
                switch (_CombineWordType)
                {
                    case eCombineWordType.WarrantAndParcel:
                        task = new TaskCombineWarrantAndParcelWord();
                        break;

                    case eCombineWordType.RegisterBookAndParcel:
                        task = new TaskCombineRegisterBookAndParcelWord();
                        break;

                    case eCombineWordType.ExtendWarrantAndParcel:
                        task = new TaskCombineExtendWarrantAndParcelWord();
                        break;

                    default:
                        break;
                }
                task.Argument = new TaskCombineWordArgument()
                {
                    SourceFolder = item.Key,
                    SaveFolder = item.Value,
                    KeepSourceFormat = _Args.KeepSourceFormat
                };

                Directory.CreateDirectory(item.Value);

                this.Add(task);
            }
            base.OnGo();
        }

        #endregion Methods - Override

        #region Methods - Private

        private bool ValidateArgs()
        {
            _Args = Argument as TaskCombineWordGroupArgument;

            _ZoneLevel = _Args.ZoneLevel;
            _CombineWordType = _Args.CombineWordType;

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

        #endregion Methods - Private

        #endregion Methods
    }
}