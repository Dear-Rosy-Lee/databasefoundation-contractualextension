using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.CombineWordTask
{
    [TaskDescriptor(IsLanguageName = false, Name = "合并权证与地块示意图",
       Gallery = "成果资料数据处理",
       UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
       UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskCombineWarrantAndParcelWord : TaskCombineWord
    {
        #region Fileds

        private const string WARRANT_SUFFIX = "农村土地承包经营权证书";

        #endregion

        #region Ctor

        public TaskCombineWarrantAndParcelWord() : base()
        {
            Name = "合并权证与地块示意图";
            Description = "合并权证与地块示意图";
        }

        #endregion

        #region Methods - Override

        protected override Dictionary<int, FileInfo> GetSourceDictionary(FileInfo[] fileInfos)
        {
            return fileInfos.Where(t => t.Name.EndsWith(WARRANT_SUFFIX + t.Extension))
                      .ToDictionary(key => Convert.ToInt32(key.Name.Split(SPLIT)[0]), value => value);
        }

        protected override Dictionary<int, FileInfo> GetAppendDictionayr(FileInfo[] fileInfos)
        {
            return fileInfos.Where(t => t.Name.StartsWith(LANDPARCEL_PREFIX))
                   .ToDictionary(key => Convert.ToInt32(key.Name.Substring(19, 4)), value => value);
        }

        #endregion
    }
}
