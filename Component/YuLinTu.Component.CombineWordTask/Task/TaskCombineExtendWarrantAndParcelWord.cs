using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.CombineWordTask
{
    [TaskDescriptor(IsLanguageName = false, Name = "合并权证与地块示意图扩展页",
       Gallery = "成果资料数据处理",
       UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
       UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskCombineExtendWarrantAndParcelWord : TaskCombineWord
    {
        #region Fileds

        private const string WARRANT_SUFFIX = "农村土地承包经营权证地块扩展";
        private const string EXTEND_WARRANT_SUFFIX = "农村土地承包经营权标准地块示意图扩展";

        protected override string APPEND_FILE { get; set; } = "地块示意图扩展页";

        #endregion Fileds

        #region Ctor

        public TaskCombineExtendWarrantAndParcelWord() : base()
        {
            Name = "合并权证与地块示意图扩展页";
            Description = "合并权证与地块示意图扩展页";
        }

        #endregion Ctor

        #region Methods - Override

        protected override Dictionary<int, FileInfo> GetSourceDictionary(FileInfo[] fileInfos)
        {
            return fileInfos.Where(t => t.Name.EndsWith(WARRANT_SUFFIX + t.Extension))
                      .ToDictionary(key => Convert.ToInt32(key.Name.Split(SPLIT)[0]), value => value);
        }

        protected override Dictionary<int, FileInfo> GetAppendDictionayr(FileInfo[] fileInfos)
        {
            return fileInfos.Where(t => t.Name.EndsWith(EXTEND_WARRANT_SUFFIX + t.Extension))
                       .ToDictionary(key => Convert.ToInt32(key.Name.Split(SPLIT)[0]), value => value);
        }

        #endregion Methods - Override
    }
}